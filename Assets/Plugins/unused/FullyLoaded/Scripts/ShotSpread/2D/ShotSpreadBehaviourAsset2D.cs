using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public abstract class ShotSpreadBehaviourAsset2D : CustomBehaviourAsset, ICustomBehaviourSettingsSpawner<ShotSpreadBehaviourSettings2D>
    {
        // -------------------------------------------------------------------------------------------------

        public abstract ShotSpreadBehaviourSettings2D CreateBehaviourSettingsInstance();

        // -------------------------------------------------------------------------------------------------

        public abstract Vector2 GetRandomizedDirection(Vector2 direction,
                                                       int shotIndex,
                                                       int shotCount,
                                                       ShotSpreadBehaviourSettings2D shotSpreadSettings);

        // -------------------------------------------------------------------------------------------------
    }
}
