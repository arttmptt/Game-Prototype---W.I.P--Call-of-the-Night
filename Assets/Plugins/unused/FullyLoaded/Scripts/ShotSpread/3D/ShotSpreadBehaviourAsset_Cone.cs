using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    #if FULLY_LOADED_DEV
    [CreateAssetMenu(menuName = "Fully Loaded/Shot-Spread Behaviour Asset/Cone")]
    #endif
    public class ShotSpreadBehaviourAsset_Cone : ShotSpreadBehaviourAsset
    {
        // -------------------------------------------------------------------------------------------------

        public override ShotSpreadBehaviourSettings CreateBehaviourSettingsInstance()
        {
            return new ShotSpreadBehaviourSettings_Cone();
        }

        // -------------------------------------------------------------------------------------------------

        public override Vector3 GetRandomizedDirection(Vector3 direction,
                                                       Vector3 upVector,
                                                       Vector3 rightVector,
                                                       int shotIndex,
                                                       int shotCount,
                                                       ShotSpreadBehaviourSettings shotSpreadSettings)
        {
            var settings = shotSpreadSettings as ShotSpreadBehaviourSettings_Cone;
            if (settings == null)
            {
                Debug.LogWarning("failed to cast ShotSpreadBehaviourSettings to correct type");
                return direction;
            }

            float min = Mathf.Min(settings.minAngle, 90.0f);
            float max = Mathf.Min(settings.maxAngle, 90.0f);
            if (min > max)
            {
                min = settings.maxAngle;
                max = settings.minAngle;
            }

            Quaternion rot1 = Quaternion.identity;
            if (settings.circumferentialAngleType == EShotSpreadAngle.Random)
            {
                // circumferential angle is randomized
                rot1 = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), direction);
            }
            else if (settings.circumferentialAngleType == EShotSpreadAngle.EvenlySpaced)
            {
                // shots are spaced equidistantally around circumference of the cone
                float stepAngle = 360.0f / shotCount;
                rot1 = Quaternion.AngleAxis(shotIndex * stepAngle, direction);
            }

            Quaternion rot2 = Quaternion.AngleAxis(Random.Range(min, max), rightVector);
            return (rot1 * rot2 * direction).normalized;
        }

        // -------------------------------------------------------------------------------------------------
    }
}
