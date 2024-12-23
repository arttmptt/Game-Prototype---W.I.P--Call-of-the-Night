using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public class ExampleWeaponAttachment : MonoBehaviour
    {
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private Transform m_mesh = null;
		[SerializeField] private Transform m_muzzlePoint = null;
		public Transform muzzlePoint { get { return m_muzzlePoint; } }

		private Animator m_anim = null;
		private MeshRenderer m_meshRenderer = null;
		private WeaponDefinition m_weapon = null;

		public enum EWeaponAttachmentEvent
		{
			Fire,
			Reload,
			SwapIn,
			SwapOut,
		}

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			if (m_mesh != null)
			{
				m_meshRenderer = m_mesh.GetComponent<MeshRenderer>();
			}
		}

		// -------------------------------------------------------------------------------------------------

		public void Initialize(WeaponDefinition weapon)
		{
			m_weapon = weapon;

			SetIsActive(false);
		}

		// -------------------------------------------------------------------------------------------------

		public void SetIsActive(bool active)
		{
			if (m_mesh != null)
			{
				m_mesh.gameObject.SetActive(active);
			}
		}

		// -------------------------------------------------------------------------------------------------

		public void SetMeshVisible(bool visible)
		{
			if (m_meshRenderer != null)
			{
				m_meshRenderer.enabled = visible;
			}
		}

		// -------------------------------------------------------------------------------------------------

		public void OnCameraModeChanged(bool isFirstPerson)
		{
			SetMeshVisible(isFirstPerson);
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			// for some reason the Animator parameters are not initialized until the first update
			if (m_anim == null)
			{
				m_anim = GetComponent<Animator>();
				if (m_anim != null && m_weapon != null)
				{
					float reloadTransitionTime = Mathf.Min(m_weapon.primaryFire.ammoConfig.reloadTime * 0.5f, 0.1f);
					float reloadTime = m_weapon.primaryFire.ammoConfig.reloadTime - (reloadTransitionTime * 2);

					m_anim.SetFloat("SwapInTime", Mathf.Max(0.01f, 1.0f / m_weapon.weaponSwitchConfig.switchInTime));
					m_anim.SetFloat("SwapOutTime", Mathf.Max(0.01f, 1.0f / m_weapon.weaponSwitchConfig.switchOutTime));
					m_anim.SetFloat("ReloadTransitionTime", Mathf.Max(0.01f, 1.0f / reloadTransitionTime));
					m_anim.SetFloat("ReloadTime", Mathf.Max(0.01f, 1.0f / reloadTime));
				}
			}
		}

		// -------------------------------------------------------------------------------------------------

		public void TriggerAnimEvent(EWeaponAttachmentEvent evt)
		{
			if (m_anim != null)
			{
				switch (evt)
				{
					case EWeaponAttachmentEvent.Fire:    m_anim.SetTrigger("Fire"); break;
					case EWeaponAttachmentEvent.Reload:  m_anim.SetTrigger("Reload"); break;
					case EWeaponAttachmentEvent.SwapIn:  m_anim.SetTrigger("SwapIn"); break;
					case EWeaponAttachmentEvent.SwapOut: m_anim.SetTrigger("SwapOut"); break;
				}
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
