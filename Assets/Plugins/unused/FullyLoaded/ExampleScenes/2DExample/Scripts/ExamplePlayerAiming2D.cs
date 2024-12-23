using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace FullyLoaded
{
    public class ExamplePlayerAiming2D : MonoBehaviour
    {
        // -------------------------------------------------------------------------------------------------

        [SerializeField] private Transform m_rotationTransform = null;
		[SerializeField] private Texture2D m_cursorTexture = null;
		[SerializeField] private bool m_showCursor = true;

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			Cursor.visible = (m_showCursor && m_cursorTexture != null);

			if (m_cursorTexture != null)
			{
				Vector2 offset = new Vector2(m_cursorTexture.width / 2, m_cursorTexture.height / 2);
				Cursor.SetCursor(m_cursorTexture, offset, CursorMode.Auto);
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			if (m_rotationTransform != null)
			{
				// read mouse position from input
				Vector2 mousePos = Vector2.zero;
				#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
				mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
				#elif ENABLE_LEGACY_INPUT_MANAGER
				mousePos = Input.mousePosition;
				#endif

				// convert to world position
				Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
				mouseWorldPos.z = 0.0f;

				// calculate direction and rotate the transform to face it
				Vector2 aimDir = (mouseWorldPos - m_rotationTransform.position).normalized;
				float aimAngle = Vector2.SignedAngle(Vector2.right, aimDir);
				m_rotationTransform.rotation = Quaternion.Euler(0.0f, 0.0f, aimAngle);
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
