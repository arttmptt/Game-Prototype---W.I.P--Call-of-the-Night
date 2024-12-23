using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class AmmoConfig
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private int m_ammoUsedPerActivation = 1;
		[SerializeField] private int m_ammoUsedPerShot = 0;
		[SerializeField] private AmmoType m_ammoType = null;
		[SerializeField] private bool m_isInfinite = true;

		[SerializeField] private bool m_hasReloadableClip = false;
		[SerializeField] private int m_clipSize = 1;
		[SerializeField] private float m_reloadTime = 0.0f;
		[SerializeField] private bool m_allowManualReload = true;
		[SerializeField] private bool m_allowAutoReload = true;
		[SerializeField] private bool m_disableWeaponDuringAutoReload = true;

		public int ammoUsedPerActivation { get { return Mathf.Max(0, m_ammoUsedPerActivation); } }
		public int ammoUsedPerShot { get { return Mathf.Max(0, m_ammoUsedPerShot); } }
		public AmmoType ammoType { get { return m_ammoType; } }
		public bool isInfinite { get { return m_isInfinite; } }
		public bool hasReloadableClip { get { return m_hasReloadableClip; } }
		public int clipSize { get { return Mathf.Max(0, m_clipSize); } }
		public float reloadTime { get { return m_reloadTime; } }
		public bool allowManualReload { get { return m_allowManualReload; } }
		public bool allowAutoReload { get { return m_allowAutoReload; } }
		public bool disableWeaponDuringAutoReload { get { return m_disableWeaponDuringAutoReload; } }

		// -------------------------------------------------------------------------------------------------

		// called from PersistentWeaponState to initialize the weapon state of a newly acquired weapon
		public void InitializeWeaponState(WeaponStateInternal weaponState)
		{
			weaponState.SetAmmoStorageCapacity(hasReloadableClip ? clipSize : 0);
			weaponState.SetAmmoStorageCount(hasReloadableClip ? clipSize : 0);
		}

		// -------------------------------------------------------------------------------------------------
	}
}
