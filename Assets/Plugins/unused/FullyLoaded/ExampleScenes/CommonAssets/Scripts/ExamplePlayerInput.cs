using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace FullyLoaded
{
    public class ExamplePlayerInput : MonoBehaviour
    {
		// -------------------------------------------------------------------------------------------------

		#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
		[SerializeField] private InputAction m_primaryFireInput = null;
		[SerializeField] private InputAction m_secondaryFireInput = null;
		[SerializeField] private InputAction m_reloadInput = null;
		[SerializeField] private InputAction m_nextWeapon = null;
		[SerializeField] private InputAction m_prevWeapon = null;
		#endif

		private WeaponUser m_weaponUser = null;

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			m_weaponUser = GetComponent<WeaponUser>();

			#if ENABLE_INPUT_SYSTEM && !NEW_INPUT_SYSTEM_INSTALLED && !ENABLE_LEGACY_INPUT_MANAGER
			Debug.Log("you have the new input system enabled but not installed - no input code can run!\n");
			#endif

			#if NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
			// set up input actions
			if (m_weaponUser != null)
			{
				if (m_primaryFireInput != null)
				{
					if (m_primaryFireInput.bindings.Count == 0 || m_primaryFireInput.bindings[0].path.Length == 0)
					{
						m_primaryFireInput = new InputAction("primaryFire", binding: "<Mouse>/leftButton");
					}
					m_primaryFireInput.Enable();
					m_primaryFireInput.performed += ctx => m_weaponUser.PullTrigger(EWeaponMode.Primary);
					m_primaryFireInput.canceled += ctx => m_weaponUser.ReleaseTrigger(EWeaponMode.Primary);
				}
				if (m_secondaryFireInput != null)
				{
					if (m_secondaryFireInput.bindings.Count == 0 || m_secondaryFireInput.bindings[0].path.Length == 0)
					{
						m_secondaryFireInput = new InputAction("secondaryFire", binding: "<Mouse>/rightButton");
					}
					m_secondaryFireInput.Enable();
					m_secondaryFireInput.performed += ctx => m_weaponUser.PullTrigger(EWeaponMode.Secondary);
					m_secondaryFireInput.canceled += ctx => m_weaponUser.ReleaseTrigger(EWeaponMode.Secondary);
				}
				if (m_reloadInput != null)
				{
					if (m_reloadInput.bindings.Count == 0 || m_reloadInput.bindings[0].path.Length == 0)
					{
						m_reloadInput = new InputAction("reload", binding: "<Keyboard>/r");
					}
					m_reloadInput.Enable();
					m_reloadInput.performed += ctx => m_weaponUser.Reload();
				}
				if (m_nextWeapon != null)
				{
					if (m_nextWeapon.bindings.Count == 0 || m_nextWeapon.bindings[0].path.Length == 0)
					{
						m_nextWeapon = new InputAction("nextWeapon", binding: "<Keyboard>/e");
					}
					m_nextWeapon.Enable();
					m_nextWeapon.performed += ctx => m_weaponUser.SwitchToNextWeapon();
				}
				if (m_prevWeapon != null)
				{
					if (m_prevWeapon.bindings.Count == 0 || m_prevWeapon.bindings[0].path.Length == 0)
					{
						m_prevWeapon = new InputAction("prevWeapon", binding: "<Keyboard>/q");
					}
					m_prevWeapon.Enable();
					m_prevWeapon.performed += ctx => m_weaponUser.SwitchToPrevWeapon();
				}
			}
			#endif
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			#if ENABLE_LEGACY_INPUT_MANAGER && !(NEW_INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM)

			// respond to legacy input system
			if (m_weaponUser != null)
			{
				// primary fire
				if (Input.GetMouseButtonDown(0))
				{
					m_weaponUser.PullTrigger(EWeaponMode.Primary);
				}
				else if (Input.GetMouseButtonUp(0))
				{
					m_weaponUser.ReleaseTrigger(EWeaponMode.Primary);
				}

				// secondary fire
				if (Input.GetMouseButtonDown(1))
				{
					m_weaponUser.PullTrigger(EWeaponMode.Secondary);
				}
				else if (Input.GetMouseButtonUp(1))
				{
					m_weaponUser.ReleaseTrigger(EWeaponMode.Secondary);
				}

				// reload
				if (Input.GetKeyDown(KeyCode.R))
				{
					m_weaponUser.Reload();
				}

				// next weapon
				if (Input.GetKeyDown(KeyCode.E))
				{
					m_weaponUser.SwitchToNextWeapon();
				}

				// prev weapon
				if (Input.GetKeyDown(KeyCode.Q))
				{
					m_weaponUser.SwitchToPrevWeapon();
				}
			}

			#endif
		}

		// -------------------------------------------------------------------------------------------------
	}
}
