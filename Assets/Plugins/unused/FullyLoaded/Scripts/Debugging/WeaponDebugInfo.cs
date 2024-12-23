using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[DisallowMultipleComponent]
	public class WeaponDebugInfo : MonoBehaviour
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private bool m_debugGuiEnabled = true;
		[SerializeField] private bool m_eventLogging = false;

		private WeaponUser m_weaponUser = null;
		private IAmmoInfo m_ammoInfo = null;

		// -------------------------------------------------------------------------------------------------

		private void Start()
		{
			m_weaponUser = GetComponent<WeaponUser>();
			if (m_weaponUser != null)
			{
				m_ammoInfo = m_weaponUser.ammoInfo;
			}
			else
			{
				m_ammoInfo = GetComponent<IAmmoInfo>();
			}

			if (m_eventLogging && m_weaponUser != null && m_weaponUser.weaponEvents != null)
			{
				RegisterLoggingEvents(m_weaponUser.weaponEvents);
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void RegisterLoggingEvents(WeaponEvents events)
		{
			events.OnTriggerChanged += (weapon, mode, trigger) =>
			{
				Debug.Log($"OnTriggerChanged: <{weapon.name}: {mode}>, trigger: {trigger}\n");
			};

			events.OnActivation += (weapon, mode, success) =>
			{
				Debug.Log($"OnActivation: <{weapon.name}: {mode}>, succeeded: {success}\n");
			};

			events.OnShotsFired += (weapon, mode, success) =>
			{
				Debug.Log($"OnShotsFired: <{weapon.name}: {mode}>, succeeded: {success}\n");
			};

			events.OnReloadStarted += (weapon, mode, type, duration) =>
			{
				Debug.Log($"OnReloadStarted: <{weapon.name}: {mode}>, {type}, duration: {duration}\n");
			};

			events.OnReloadFinished += (weapon, mode) =>
			{
				Debug.Log($"OnReloadFinished: <{weapon.name}: {mode}>\n");
			};

			events.OnWeaponSwitchOutStarted += (weapon, duration) =>
			{
				Debug.Log($"OnWeaponSwitchOutStarted: {weapon.name}, {duration}\n");
			};

			events.OnWeaponSwitchOutFinished += (weapon) =>
			{
				Debug.Log($"OnWeaponSwitchOutFinished: {weapon.name}\n");
			};

			events.OnWeaponSwitchInStarted += (weapon, duration) =>
			{
				Debug.Log($"OnWeaponSwitchInStarted: {weapon.name}, {duration}\n");
			};

			events.OnWeaponSwitchInFinished += (weapon) =>
			{
				Debug.Log($"OnWeaponSwitchInFinished: {weapon.name}\n");
			};

			events.OnOwnershipChanged += (weapon, owned) =>
			{
				Debug.Log($"OnOwnershipChanged: {weapon.name}, owned: {owned}\n");
			};
		}

		// -------------------------------------------------------------------------------------------------

		private string GetAmmoDisplay(EWeaponMode weaponMode)
		{
			string text = (weaponMode == EWeaponMode.Primary) ? "Primary Ammo: " : "Secondary Ammo: ";

			if (m_weaponUser != null)
			{
				WeaponState weaponState = m_weaponUser.GetWeaponState(weaponMode);
				WeaponConfig weaponConfig = (weaponMode == EWeaponMode.Primary) ?
					m_weaponUser.currentWeapon?.primaryFire : m_weaponUser.currentWeapon?.secondaryFire;

				if (weaponConfig != null)
				{
					bool infinite = false;
					AmmoType ammoType = weaponConfig.ammoConfig.ammoType;

					if (weaponConfig.ammoConfig.isInfinite ||
						(m_ammoInfo != null && ammoType != null && m_ammoInfo.HasInfiniteAmmo(ammoType)))
					{
						infinite = true;
					}

					if (weaponState != null && weaponState.ammoStorageCapacity > 0)
					{
						if (infinite)
						{
							text += $"{weaponState.ammoStorageCount} / {weaponState.ammoStorageCapacity} (infinite)";
						}
						else
						{
							text += $"{weaponState.ammoStorageCount} / {weaponState.ammoStorageCapacity} ({m_ammoInfo.GetAmmoCount(ammoType)})";
						}
					}
					else
					{
						text += (infinite) ? "infinite" : $"{m_ammoInfo.GetAmmoCount(ammoType)}";
					}
				}
			}

			return text;
		}

		// -------------------------------------------------------------------------------------------------

		private string GetTriggerChargesDisplay(EWeaponMode weaponMode)
		{
			string text = (weaponMode == EWeaponMode.Primary) ? "Primary Trigger-Charges: " : "Secondary Trigger-Charges: ";

			if (m_weaponUser != null)
			{
				WeaponState weaponState = m_weaponUser.GetWeaponState(weaponMode);
				if (weaponState != null && weaponState.maxTriggerCharges > 0)
				{
					text += $"{weaponState.currentTriggerCharges} / {weaponState.maxTriggerCharges}";
				}
			}
			else
			{
				text += "<none>";
			}

			return text;
		}

		// -------------------------------------------------------------------------------------------------

		private void OnGUI()
		{
			if (m_debugGuiEnabled)
			{
				string color = "white";
				if (m_weaponUser.weaponSwitchInProgress)
				{
					color = "orange";
				}
				else if (m_weaponUser.reloadInProgress)
				{
					color = "yellow";
				}

				string currentWeaponName = $"<color={color}>";
				if (m_weaponUser != null)
				{
					currentWeaponName += (m_weaponUser.currentWeapon != null) ?
						m_weaponUser.currentWeapon.weaponName : "<none>";
				}
				else
				{
					currentWeaponName += "<none>";
				}
				currentWeaponName += "</color>";

				string pendingWeaponName = "<none>";
				if (m_weaponUser != null)
				{
					pendingWeaponName = (m_weaponUser.pendingWeapon != null) ?
						m_weaponUser.pendingWeapon.weaponName : "<none>";
				}

				Rect rect = new Rect(20.0f, 20.0f, Screen.width - 40.0f, Screen.height - 40.0f);
				GUI.Label(rect, new GUIContent($"current weapon: {currentWeaponName}"));
				rect.y += 16.0f;
				GUI.Label(rect, new GUIContent($"pending weapon: {pendingWeaponName}"));
				rect.y += 16.0f;
				GUI.Label(rect, new GUIContent(GetAmmoDisplay(EWeaponMode.Primary)));
				rect.y += 16.0f;
				GUI.Label(rect, new GUIContent(GetAmmoDisplay(EWeaponMode.Secondary)));
				rect.y += 16.0f;
				GUI.Label(rect, new GUIContent(GetTriggerChargesDisplay(EWeaponMode.Primary)));
				rect.y += 16.0f;
				GUI.Label(rect, new GUIContent(GetTriggerChargesDisplay(EWeaponMode.Secondary)));
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
