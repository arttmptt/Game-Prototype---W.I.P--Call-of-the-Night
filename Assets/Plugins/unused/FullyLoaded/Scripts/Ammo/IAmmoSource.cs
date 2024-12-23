using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public interface IAmmoSource: IAmmoInfo
    {
        // -------------------------------------------------------------------------------------------------

        public bool SetInfiniteAmmo(AmmoType type, bool isInfinite);
        public bool SetAmmoCount(AmmoType type, int ammoCount);
        public int AddAmmo(AmmoType type, int amount);
        public int RemoveAmmo(AmmoType type, int amount);

        // -------------------------------------------------------------------------------------------------
    }
}
