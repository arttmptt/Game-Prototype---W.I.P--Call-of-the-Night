using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[CreateAssetMenu(menuName = "Fully Loaded/Weapon Definition Asset")]
	public class WeaponDefinition : UniqueScriptableObject
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private string m_weaponName = "";
		[SerializeField] private WeaponSwitchConfig m_weaponSwitchConfig = new WeaponSwitchConfig();
		[SerializeField] private WeaponConfig m_primaryFireConfig = null;
		[SerializeField] private WeaponConfig m_secondaryFireConfig = null;
		[SerializeField] private bool m_secondaryFireEnabled = false;

		public string weaponName { get { return m_weaponName; } }
		public WeaponSwitchConfig weaponSwitchConfig { get { return m_weaponSwitchConfig; } }
		public WeaponConfig primaryFire { get { return m_primaryFireConfig; } }
		public WeaponConfig secondaryFire { get { return m_secondaryFireConfig; } }
		public bool secondaryFireEnabled { get { return m_secondaryFireEnabled; } }

		// -------------------------------------------------------------------------------------------------
	}
}
