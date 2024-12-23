using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace FullyLoaded
{
	public class ExamplePlayerCamera : MonoBehaviour
	{
		// -------------------------------------------------------------------------------------------------

		#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
		[SerializeField] private InputAction m_lookDirectionInput = null;
		[SerializeField] private InputAction m_toggleCameraInput = null;
		#endif

		[SerializeField] private float m_lookSensitivity = 0.2f;
		[SerializeField] private bool m_firstPerson = true;
		[SerializeField] private bool m_invertLook = false;
		[SerializeField] private Camera m_firstPersonCamera = null;
		[SerializeField] private Camera m_thirdPersonCamera = null;
		[SerializeField] private Transform m_meshTransform = null;

		public float pitchAngle { get { return m_pitchAngle; } }
		public float yawAngle { get { return m_yawAngle; } }
		public bool isFirstPerson { get { return m_firstPerson; } }

		private Transform m_viewpointTransform = null;
		private float m_pitchAngle = 0.0f;
		private float m_yawAngle = 0.0f;
		private WeaponUser m_weaponUser = null;

		public delegate void CameraModeChanged(bool isFirstPerson);
		public event CameraModeChanged OnCameraModeChanged = null;

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			m_viewpointTransform = transform;
			m_weaponUser = GetComponentInParent<WeaponUser>();

			#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
			// if no bindings are set, add some defaults at runtime
			if (m_lookDirectionInput.bindings.Count == 0 || m_lookDirectionInput.bindings[0].path.Length == 0)
			{
				m_lookDirectionInput = new InputAction("lookDir", binding: "<Mouse>/delta");
			}
			if (m_toggleCameraInput.bindings.Count == 0 || m_toggleCameraInput.bindings[0].path.Length == 0)
			{
				m_toggleCameraInput = new InputAction("toggleCamera", binding: "<Keyboard>/space");
			}

			m_lookDirectionInput?.Enable();
			m_toggleCameraInput?.Enable();
			#endif

			Cursor.lockState = CursorLockMode.Locked;
		}

		// -------------------------------------------------------------------------------------------------

		private void Start()
		{
			if (m_weaponUser != null)
			{
				m_weaponUser.weaponAim.SetAimMode(EWeaponAimMode.UseTargetPosition);
			}

			SwitchCamera();
		}

		// -------------------------------------------------------------------------------------------------

		private void SwitchCamera()
		{
			// switch between the two cameras
			if (m_firstPersonCamera != null)
			{
				m_firstPersonCamera.enabled = m_firstPerson;
			}
			if (m_thirdPersonCamera != null)
			{
				m_thirdPersonCamera.enabled = !m_firstPerson;
			}

			// disable the player mesh in first person mode
			if (m_meshTransform != null)
			{
				m_meshTransform.gameObject.SetActive(!m_firstPerson);
			}

			// notify anything else that cares about camera modes
			if (OnCameraModeChanged != null)
			{
				OnCameraModeChanged(m_firstPerson);
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			// check for camera mode toggle (first person / third person)
			bool toggleInput = false;
			#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
			if (m_toggleCameraInput != null)
			{
				toggleInput = (m_toggleCameraInput.triggered && m_toggleCameraInput.ReadValue<float>() > 0.0f);
			}
			#elif ENABLE_LEGACY_INPUT_MANAGER
			toggleInput = Input.GetButtonDown("Jump");
			#endif

			if (toggleInput)
			{
				m_firstPerson = !m_firstPerson;
				SwitchCamera();
			}

			if (m_viewpointTransform != null)
			{
				float sensitivity = m_lookSensitivity;

				Vector2 delta = Vector2.zero;
				#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
				delta = Vector2.zero;
				if (m_lookDirectionInput != null)
				{
					delta = m_lookDirectionInput.ReadValue<Vector2>();
				}
				#elif ENABLE_LEGACY_INPUT_MANAGER
				delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
				sensitivity = m_lookSensitivity * 10.0f;
				#endif

				// apply look sensitivity
				delta *= sensitivity;

				// orient the viewpoint
				float yaw = m_viewpointTransform.eulerAngles.y + delta.x;
				float pitch = m_viewpointTransform.eulerAngles.x;
				if (pitch > 90.0f)
				{
					pitch -= 360.0f;
				}
				float pitchDelta = m_invertLook ? delta.y : -delta.y;
				pitch = Mathf.Clamp(pitch + pitchDelta, -89.0f, 89.0f);

				m_viewpointTransform.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
				m_pitchAngle = pitch;
				m_yawAngle = yaw;

				// update the weapon aim
				// we use 'target position' aim mode, and raycast through the screen at the crosshair
				// to determine where our bullet should go
				if (m_weaponUser != null)
				{
					Camera cam = (m_firstPerson) ? m_firstPersonCamera : m_thirdPersonCamera;
					if (cam != null)
					{
						Vector3 screenPoint = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
						Ray ray = cam.ScreenPointToRay(screenPoint);

						RaycastHit hit;
						if (Physics.Raycast(ray, out hit))
						{
							m_weaponUser.weaponAim.SetTargetPosition(hit.point);
						}
						else
						{
							Vector3 worldPoint = cam.ScreenToWorldPoint(screenPoint)
								+ (ray.direction * 50.0f);
							m_weaponUser.weaponAim.SetTargetPosition(worldPoint);
						}
					}
				}
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
