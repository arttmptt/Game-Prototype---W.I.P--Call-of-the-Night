using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    #if FULLY_LOADED_DEV
    [CreateAssetMenu(menuName = "Fully Loaded/Shot-Spread Behaviour Asset/Box")]
    #endif
    public class ShotSpreadBehaviourAsset_Box : ShotSpreadBehaviourAsset
    {
        // -------------------------------------------------------------------------------------------------

        public override ShotSpreadBehaviourSettings CreateBehaviourSettingsInstance()
        {
            return new ShotSpreadBehaviourSettings_Box();
        }

        // -------------------------------------------------------------------------------------------------

        public override Vector3 GetRandomizedDirection(Vector3 direction,
                                                       Vector3 upVector,
                                                       Vector3 rightVector,
                                                       int shotIndex,
                                                       int shotCount,
                                                       ShotSpreadBehaviourSettings shotSpreadSettings)
        {
            var settings = shotSpreadSettings as ShotSpreadBehaviourSettings_Box;
            if (settings == null)
            {
                Debug.LogWarning("failed to cast ShotSpreadBehaviourSettings to correct type");
                return direction;
            }

            Quaternion horizontalRot = GetAngleForAxis(settings.minHorizontalAngle,
                                                       settings.maxHorizontalAngle,
                                                       settings.horizontalAngleType,
                                                       upVector,
                                                       shotIndex,
                                                       shotCount);

            Quaternion verticalRot = GetAngleForAxis(settings.minVerticalAngle,
                                                     settings.maxVerticalAngle,
                                                     settings.verticalAngleType,
                                                     rightVector,
                                                     shotIndex,
                                                     shotCount);

            return (horizontalRot * verticalRot * direction).normalized;
        }

        // -------------------------------------------------------------------------------------------------

        private Quaternion GetAngleForAxis(float minAngle,
                                           float maxAngle,
                                           EShotSpreadAngle angleType,
                                           Vector3 rotationAxis,
                                           int shotIndex,
                                           int shotCount)
        {
            Quaternion rot = Quaternion.identity;

            float min = minAngle;
            float max = maxAngle;
            if (min > max)
            {
                min = maxAngle;
                max = minAngle;
            }

            if (angleType == EShotSpreadAngle.Random)
            {
                // angle is randomized between min and max
                // negated so that positive angle is up
                rot = Quaternion.AngleAxis(-Random.Range(min, max), rotationAxis);
            }
            else if (angleType == EShotSpreadAngle.EvenlySpaced)
            {
                // shots are spaced equidistantally between min and max angle, based upon their shot index
                if (shotCount > 1)
                {
                    float angleRange = maxAngle - minAngle;
                    float stepAngle = angleRange / (shotCount - 1);
                    rot = Quaternion.AngleAxis(minAngle + (shotIndex * stepAngle), rotationAxis);
                }
            }

            return rot;
        }

        // -------------------------------------------------------------------------------------------------
    }
}
