using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public interface ITriggerBehaviourAmmoInterface
    {
        bool CheckHasSufficientAmmo(int ammoNeeded);
        bool ConsumeAmmo(int ammoNeeded);
    }
}
