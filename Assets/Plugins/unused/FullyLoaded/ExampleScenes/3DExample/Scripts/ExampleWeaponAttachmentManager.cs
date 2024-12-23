using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	using EWeaponAttachmentEvent = ExampleWeaponAttachment.EWeaponAttachmentEvent;

	public class ExampleWeaponAttachmentManager : MonoBehaviour
    {
		// -------------------------------------------------------------------------------------------------

		[System.Serializable]
		public struct WeaponAttachmentSetup
		{
			[SerializeField] private WeaponDefinition m_weapon;
			[SerializeField] private ExampleWeaponAttachment m_attachment;

			public WeaponDefinition weapon { get { return m_weapon; } }
			public ExampleWeaponAttachment attachment { get { return m_attachment; } }
		}

		[SerializeField] private List<WeaponAttachmentSetup> m_weapons = new List<WeaponAttachmentSetup>();

		private WeaponUser m_weaponUser = null;
		private ExamplePlayerCamera m_camera = null;
		private WeaponDefinition m_currentWeapon = null;
		private Dictionary<WeaponDefinition, ExampleWeaponAttachment> m_attachments = new Dictionary<WeaponDefinition, ExampleWeaponAttachment>();

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			// register for weapon events
			m_weaponUser = GetComponentInParent<WeaponUser>();
			if (m_weaponUser != null)
			{
				m_weaponUser.weaponEvents.OnWeaponSwitchInStarted += OnSwitchInStarted;
				m_weaponUser.weaponEvents.OnWeaponSwitchOutStarted += OnSwitchOutStarted;
				m_weaponUser.weaponEvents.OnWeaponSwitchOutFinished += OnSwitchOutFinished;
				m_weaponUser.weaponEvents.OnShotsFired += OnShotsFired;
				m_weaponUser.weaponEvents.OnReloadStarted += OnReloadStarted;
			}

			// construct the dictionary
			for (int i = 0; i < m_weapons.Count; ++i)
			{
				if (m_weapons[i].weapon != null && m_weapons[i].attachment != null)
				{
					if (!m_attachments.ContainsKey(m_weapons[i].weapon))
					{
						m_attachments.Add(m_weapons[i].weapon, m_weapons[i].attachment);
						m_weapons[i].attachment.Initialize(m_weapons[i].weapon);
					}
				}
			}

			// register for camera mode change events
			m_camera = GetComponentInParent<ExamplePlayerCamera>();
			if (m_camera != null)
			{
				m_camera.OnCameraModeChanged += isFirstPerson =>
				{
					if (m_currentWeapon != null && m_attachments.ContainsKey(m_currentWeapon))
					{
						m_attachments[m_currentWeapon].OnCameraModeChanged(isFirstPerson);
					}
				};
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void TriggerAnimation(WeaponDefinition weapon, EWeaponAttachmentEvent animEvent)
		{
			if (weapon != null && m_attachments.ContainsKey(weapon))
			{
				m_attachments[weapon].TriggerAnimEvent(animEvent);
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void OnSwitchInStarted(WeaponDefinition weapon, float duration)
		{
			if (m_currentWeapon != weapon)
			{
				if (weapon != null && m_attachments.ContainsKey(weapon))
				{
					m_attachments[weapon].SetIsActive(true);
					m_attachments[weapon].SetMeshVisible(m_camera.isFirstPerson);

					if (m_weaponUser != null)
					{
						m_weaponUser.weaponAim.SetMuzzlePoint(m_attachments[weapon].muzzlePoint);
					}
				}

				m_currentWeapon = weapon;
			}

			TriggerAnimation(weapon, EWeaponAttachmentEvent.SwapIn);
		}

		// -------------------------------------------------------------------------------------------------

		private void OnSwitchOutStarted(WeaponDefinition weapon, float duration)
		{
			TriggerAnimation(weapon, EWeaponAttachmentEvent.SwapOut);
		}

		// -------------------------------------------------------------------------------------------------

		private void OnSwitchOutFinished(WeaponDefinition weapon)
		{
			if (weapon != null && m_attachments.ContainsKey(weapon))
			{
				m_attachments[weapon].SetIsActive(false);
				m_attachments[weapon].SetMeshVisible(m_camera.isFirstPerson);
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void OnShotsFired(WeaponDefinition weapon, EWeaponMode mode, bool success)
		{
			TriggerAnimation(weapon, EWeaponAttachmentEvent.Fire);
		}

		// -------------------------------------------------------------------------------------------------

		private void OnReloadStarted(WeaponDefinition weapon,
			                         EWeaponMode mode,
									 EReloadType reloadType,
									 float reloadDuration)
		{
			TriggerAnimation(weapon, EWeaponAttachmentEvent.Reload);
		}

		// -------------------------------------------------------------------------------------------------
	}
}
