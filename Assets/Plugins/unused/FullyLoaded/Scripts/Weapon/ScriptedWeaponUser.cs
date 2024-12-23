using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace FullyLoaded
{
	[DisallowMultipleComponent]
	public class ScriptedWeaponUser : BaseWeaponUser
	{
		// -------------------------------------------------------------------------------------------------

		private WeaponDefinition m_currentWeapon = null;
		public override WeaponDefinition currentWeapon { get { return m_currentWeapon; } }

		// -------------------------------------------------------------------------------------------------

		protected override void Awake()
		{
			base.Awake();

			m_currentWeapon = m_initialWeapon;

			// create and initialize the weapon instance
			m_weaponInstance = new WeaponInstance(this, weaponAim, null, null);
			if (m_currentWeapon != null)
			{
				m_weaponInstance.Reset(weaponAim, null, m_currentWeapon, null, null);
				m_weaponInstance.SetIsEquipped(true);
			}
		}

		// -------------------------------------------------------------------------------------------------

		public override bool SwitchToWeapon(WeaponDefinition newWeapon)
		{
			if (newWeapon != m_currentWeapon)
			{
				m_currentWeapon = newWeapon;
				m_weaponInstance?.Reset(weaponAim, null, m_currentWeapon, null, null);
				m_weaponInstance?.SetIsEquipped(m_currentWeapon != null);
				return true;
			}

			return false;
		}

		// -------------------------------------------------------------------------------------------------

		public void Fire(EWeaponMode weaponMode)
		{
			m_weaponInstance?.Activate(weaponMode, true);
		}

		// -------------------------------------------------------------------------------------------------
	}
}
