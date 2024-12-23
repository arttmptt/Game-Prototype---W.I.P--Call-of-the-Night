using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace FullyLoaded
{
    public class ExamplePlayerMovement2D : MonoBehaviour
    {
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private Transform m_graphicsTransform = null;
		[SerializeField] private float m_speed = 1.0f;

		#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
		[SerializeField] private InputAction m_directionInput = null;
		#endif

		private Rigidbody2D m_rigidbody = null;
		private Vector3 m_graphicalPos = Vector3.zero;
		private Vector3 m_physicsPos = Vector3.zero;
		private float m_facingAngle = 0.0f;
		private float m_targetFacingAngle = 0.0f;

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			m_rigidbody = GetComponent<Rigidbody2D>();

			m_graphicalPos = transform.position;
			m_physicsPos = transform.position;

			#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
			if (m_directionInput != null)
			{
				// if no bindings are set, add some defaults at runtime
				if (m_directionInput.bindings.Count == 0 || m_directionInput.bindings[0].path.Length == 0)
				{
					m_directionInput = new InputAction("moveDir");
					m_directionInput.AddCompositeBinding("2DVector")
						.With("Up", "<Keyboard>/w")
						.With("Down", "<Keyboard>/s")
						.With("Left", "<Keyboard>/a")
						.With("Right", "<Keyboard>/d");
				}

				m_directionInput?.Enable();
			}
			#endif
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			// read directional input
			Vector2 dir = Vector2.zero;
			#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
			dir = m_directionInput.ReadValue<Vector2>().normalized;
			#elif ENABLE_LEGACY_INPUT_MANAGER
			dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
			#endif

			// rotate the player to face the last direction
			if (dir.sqrMagnitude != 0.0f)
			{
				m_targetFacingAngle = Vector2.SignedAngle(Vector2.right, dir);
			}

			m_facingAngle = Mathf.LerpAngle(m_facingAngle, m_targetFacingAngle, Time.deltaTime * 10.0f);
			m_graphicsTransform.rotation = Quaternion.Euler(0.0f, 0.0f, m_facingAngle);

			// apply input direction to the graphical position
			m_graphicalPos += (Vector3)dir * m_speed * Time.deltaTime;
			m_graphicalPos.z = 0.0f;
			m_graphicsTransform.position = m_graphicalPos;
		}

		// -------------------------------------------------------------------------------------------------

		private void FixedUpdate()
		{
			if (m_graphicsTransform != null)
			{
				m_graphicsTransform.localPosition = Vector3.zero;
			}

			m_physicsPos = m_graphicalPos;
			m_rigidbody.MovePosition(m_physicsPos);
		}

		// -------------------------------------------------------------------------------------------------
	}
}
