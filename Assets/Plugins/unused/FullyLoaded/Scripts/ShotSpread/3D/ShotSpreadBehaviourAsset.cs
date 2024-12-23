using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public abstract class ShotSpreadBehaviourAsset : CustomBehaviourAsset, ICustomBehaviourSettingsSpawner<ShotSpreadBehaviourSettings>
    {
        // -------------------------------------------------------------------------------------------------

        public abstract ShotSpreadBehaviourSettings CreateBehaviourSettingsInstance();

        // -------------------------------------------------------------------------------------------------

        public abstract Vector3 GetRandomizedDirection(Vector3 direction,
                                                       Vector3 upVector,
                                                       Vector3 rightVector,
                                                       int shotIndex,
                                                       int shotCount,
                                                       ShotSpreadBehaviourSettings shotSpreadSettings);

        // -------------------------------------------------------------------------------------------------
    }
}
