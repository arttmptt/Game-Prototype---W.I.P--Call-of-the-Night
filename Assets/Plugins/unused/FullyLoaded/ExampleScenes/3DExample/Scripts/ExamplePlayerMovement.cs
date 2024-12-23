using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace FullyLoaded
{
    public class ExamplePlayerMovement : MonoBehaviour
    {
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private Transform m_graphicalTransform = null;
		[SerializeField] protected SphereCollider m_groundCollider = null;
		[SerializeField] protected SphereCollider m_overlapCollider = null;
		[SerializeField] protected LayerMask m_overlapLayers = new LayerMask();

		#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
		[SerializeField] private InputAction m_directionInput = null;
		#endif

		[SerializeField] private float m_maxForwardSpeed = 8.0f;
		[SerializeField] private float m_maxBackwardsSpeed = 8.0f;
		[SerializeField] private float m_maxStrafeSpeed = 8.0f;
		[SerializeField] private float m_gravity = -9.8f;

		private Rigidbody m_rigidbody = null;
		private ExamplePlayerCamera m_playerCamera = null;
		private ExampleOverlapTest m_overlapTest = null;

		private Vector3 m_velocity = Vector3.zero;
		private bool m_grounded = false;
		private Vector3 m_logicalPosition = Vector3.zero;
		private Vector3 m_graphicalPosition = Vector3.zero;

		public Vector3 logicalPosition { get { return m_logicalPosition; } }
		public Vector3 graphicalPosition { get { return m_graphicalPosition; } }

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			m_rigidbody = GetComponent<Rigidbody>();
			m_playerCamera = GetComponentInChildren<ExamplePlayerCamera>();

			#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
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
			#endif

			m_logicalPosition = transform.position;
			m_graphicalPosition = transform.position;

			// initialise the overlap test
			if (m_overlapCollider != null && m_groundCollider != null)
			{
				m_groundCollider.enabled = false;
				m_groundCollider.isTrigger = true;
				m_overlapCollider.enabled = false;
				m_overlapCollider.isTrigger = true;
				m_overlapTest = new ExampleOverlapTest(m_groundCollider, m_overlapCollider, m_overlapLayers, transform);
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			Vector3 newPos = m_graphicalPosition;

			// move the pending position based upon input
			newPos = GetPositionAfterInputs(newPos);
			if (m_overlapTest.CheckForOverlap(newPos))
			{
				newPos = m_graphicalPosition;
			}

			// update the graphical position
			m_graphicalPosition = newPos;
			if (m_graphicalTransform != null)
			{
				m_graphicalTransform.position = m_graphicalPosition;
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void FixedUpdate()
		{
			m_logicalPosition = m_graphicalPosition;

			// convert the current graphical position to a local offset from the new position, so when 
			// we update the logical position the graphics remain in the same place as before
			if (m_graphicalTransform != null)
			{
				m_graphicalTransform.localPosition = m_graphicalTransform.position - m_logicalPosition;
			}

			// actually move the physics
			m_rigidbody.MovePosition(m_logicalPosition);
		}

		// -------------------------------------------------------------------------------------------------

		private Vector3 GetPositionAfterInputs(Vector3 startingPosition)
		{
			// read input
			Vector2 inputDirection = Vector2.zero;
			#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
			inputDirection = m_directionInput.ReadValue<Vector2>().normalized;
			#elif ENABLE_LEGACY_INPUT_MANAGER
			inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			if (inputDirection.sqrMagnitude > 0.0f)
			{
				inputDirection.Normalize();
			}
			#endif

			// limit directional input by max speeds, and rotate it to match world space orientation
			Vector3 scaledDirection = inputDirection;
			scaledDirection.y *= (scaledDirection.y > 0.0f) ? m_maxForwardSpeed : m_maxBackwardsSpeed;
			scaledDirection.x *= m_maxStrafeSpeed;
			Quaternion rot = Quaternion.Euler(0.0f, m_playerCamera.yawAngle, 0.0f);
			scaledDirection = rot * new Vector3(scaledDirection.x, 0.0f, scaledDirection.y);

			if (!m_grounded)
			{
				// apply gravity acceleration while not grounded
				m_velocity.y -= Mathf.Abs(m_gravity) * Time.deltaTime;
			}
			else
			{
				// apply input direction to velocity
				if (inputDirection.magnitude > 0.1f)
				{
					m_velocity = scaledDirection;
				}
				else
				{
					m_velocity = Vector3.zero;
				}
			}

			Vector3 newPos = startingPosition + (m_velocity * Time.deltaTime);

			// while grounded or falling, do a ground check to find the correction needed to snap us to ground
			if (m_grounded || m_velocity.y < 0.0f)
			{
				float correction = 0.0f;
				Vector3 groundNormal = Vector3.up;
				bool newGrounded = m_overlapTest.FindGround(newPos, 2.0f, out groundNormal, out correction);
				if (newGrounded != m_grounded)
				{
					m_grounded = newGrounded;
					if (m_grounded)
					{
						m_velocity.y = 0.0f;
					}
				}

				// apply ground correction
				if (m_grounded && correction != 0.0f)
				{
					newPos.y += correction;
				}
			}

			return newPos;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
