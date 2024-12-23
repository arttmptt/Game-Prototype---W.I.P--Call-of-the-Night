using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace FullyLoaded
{
	[DisallowMultipleComponent]
	public abstract class BaseWeaponUser : MonoBehaviour
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private Transform m_muzzlePoint = null;
		[SerializeField] private EWeaponAimMode m_aimMode = EWeaponAimMode.UseMuzzleDirection;
		[SerializeField] protected WeaponDefinition m_initialWeapon = null;

		private WeaponAim m_weaponAim = null;

		protected WeaponInstance m_weaponInstance = null;

		public abstract WeaponDefinition currentWeapon { get; }
		public WeaponAim weaponAim { get { return m_weaponAim; } }

		// -------------------------------------------------------------------------------------------------

		protected virtual void Awake()
		{
			if (m_muzzlePoint == null)
			{
				m_muzzlePoint = transform;
			}

			m_weaponAim = new WeaponAim(m_aimMode, m_muzzlePoint);
		}

		// -------------------------------------------------------------------------------------------------

		protected virtual void Update()
		{
			m_weaponInstance?.UpdateWeaponInstance(Time.time, Time.deltaTime);
		}

		// -------------------------------------------------------------------------------------------------

		public abstract bool SwitchToWeapon(WeaponDefinition newWeapon);

		// -------------------------------------------------------------------------------------------------
	}
}
