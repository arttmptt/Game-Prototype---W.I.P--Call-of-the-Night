using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public class WeaponPickup : PickupBase
    {
        // -------------------------------------------------------------------------------------------------

        [SerializeField] private WeaponDefinition m_weapon = null;

        // -------------------------------------------------------------------------------------------------

        protected override bool CollectItem(GameObject collector)
        {
            if (m_weapon != null)
            {
                WeaponBag weaponBag = collector.GetComponentInParent<WeaponBag>();
                if (weaponBag != null)
                {
                    return weaponBag.SetOwnership(m_weapon, true);
                }
            }

            return false;
        }

        // -------------------------------------------------------------------------------------------------
    }
}
