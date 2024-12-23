using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public class AmmoPickup : PickupBase
    {
        // -------------------------------------------------------------------------------------------------

        [SerializeField] private AmmoType m_ammoType = null;
        [SerializeField, Min(0)] private int m_count = 0;

        // -------------------------------------------------------------------------------------------------

        protected override bool CollectItem(GameObject collector)
        {
            if (m_ammoType != null && m_count > 0)
            {
                AmmoBelt ammoBelt = collector.GetComponentInParent<AmmoBelt>();
                if (ammoBelt != null)
                {
                    return (ammoBelt.AddAmmo(m_ammoType, m_count) > 0);
                }
            }

            return false;
        }

        // -------------------------------------------------------------------------------------------------
    }
}
