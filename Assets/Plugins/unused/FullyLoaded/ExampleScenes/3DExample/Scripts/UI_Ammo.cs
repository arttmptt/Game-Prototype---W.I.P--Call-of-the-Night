using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FullyLoaded
{
    public class UI_Ammo : MonoBehaviour
    {
        // -------------------------------------------------------------------------------------------------

        [SerializeField] private EWeaponMode m_mode = EWeaponMode.Primary;
        
		private Text m_text = null;
		private WeaponUser m_weaponUser = null;
		private IAmmoSource m_ammoSource = null;

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			m_text = GetComponent<Text>();

			// find the player's ammo source and weapon user
			GameObject obj = GameObject.FindGameObjectWithTag("Player");
			if (obj != null)
			{
				m_weaponUser = obj.GetComponent<WeaponUser>();
				m_ammoSource = obj.GetComponent<IAmmoSource>();
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			string text = "";

			if (m_weaponUser != null && m_weaponUser.currentWeapon != null)
			{
				WeaponConfig config = (m_mode == EWeaponMode.Primary) ?
					m_weaponUser.currentWeapon.primaryFire : m_weaponUser.currentWeapon.secondaryFire;

				AmmoType ammoType = config.ammoConfig.ammoType;

				// check for infinite ammo
				bool infinite = config.ammoConfig.isInfinite;
				if (m_ammoSource != null && config.ammoConfig.ammoType != null)
				{
					if (m_ammoSource.HasInfiniteAmmo(config.ammoConfig.ammoType))
					{
						infinite = true;
					}
				}

				// check for clip
				if (config.ammoConfig.hasReloadableClip)
				{
					int clipCurrent = 0;
					WeaponState state = m_weaponUser.GetWeaponState(m_mode);
					if (state != null)
					{
						clipCurrent = state.ammoStorageCount;
					}

					int supply = (m_ammoSource != null) ? m_ammoSource.GetAmmoCount(ammoType) : 0;
					text = (infinite) ? $"{clipCurrent} / Inf" : $"{clipCurrent} / {supply}";
				}
				else
				{
					int count = (m_ammoSource != null) ? m_ammoSource.GetAmmoCount(ammoType) : 0;
					text = (infinite) ? "Inf" : $"{count}";
				}
			}

			// update the UI
			if (m_text != null)
			{
				m_text.text = text;
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
