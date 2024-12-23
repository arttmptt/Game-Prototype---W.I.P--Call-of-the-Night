using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public class InfiniteAmmoSource: IAmmoSource
    {
        // -------------------------------------------------------------------------------------------------

        public int GetAmmoCount(AmmoType type)
        {
            return 0;
        }

        // -------------------------------------------------------------------------------------------------

        public int GetMaxAmmoCount(AmmoType type)
        {
            return 0;
        }

        // -------------------------------------------------------------------------------------------------

        public bool HasInfiniteAmmo(AmmoType type)
        {
            return true;
        }

        // -------------------------------------------------------------------------------------------------

        public bool SetInfiniteAmmo(AmmoType type, bool isInfinite)
        {
            return true;
        }

        // -------------------------------------------------------------------------------------------------

        public bool SetAmmoCount(AmmoType type, int ammoCount)
        {
            return true;
        }

        // -------------------------------------------------------------------------------------------------

        public int AddAmmo(AmmoType type, int amount)
        {
            return amount;
        }

        // -------------------------------------------------------------------------------------------------

        public int RemoveAmmo(AmmoType type, int amount)
        {
            return amount;
        }

        // -------------------------------------------------------------------------------------------------
    }
}
