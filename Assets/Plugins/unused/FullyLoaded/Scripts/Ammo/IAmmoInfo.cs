using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public interface IAmmoInfo
    {
        // -------------------------------------------------------------------------------------------------

        public int GetAmmoCount(AmmoType type);
        public int GetMaxAmmoCount(AmmoType type);
        public bool HasInfiniteAmmo(AmmoType type);

        // -------------------------------------------------------------------------------------------------
    }
}
