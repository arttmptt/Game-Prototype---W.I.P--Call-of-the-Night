using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
#if FULLY_LOADED_DEV
    [CreateAssetMenu(menuName = "Fully Loaded/Shot-Spread Behaviour Asset/Cone (2D)")]
#endif
    public class ShotSpreadBehaviourAsset_Cone2D : ShotSpreadBehaviourAsset2D
    {
        // -------------------------------------------------------------------------------------------------

        public override ShotSpreadBehaviourSettings2D CreateBehaviourSettingsInstance()
        {
            return new ShotSpreadBehaviourSettings_Cone2D();
        }

        // -------------------------------------------------------------------------------------------------

        public override Vector2 GetRandomizedDirection(Vector2 direction,
                                                       int shotIndex,
                                                       int shotCount,
                                                       ShotSpreadBehaviourSettings2D shotSpreadSettings)
        {
            var settings = shotSpreadSettings as ShotSpreadBehaviourSettings_Cone2D;
            if (settings == null)
            {
                Debug.LogWarning("failed to cast ShotSpreadBehaviourSettings2D to correct type");
                return direction;
            }

            if (settings.angleType == EShotSpreadAngle.Random)
            {
                // pick a random angle between zero and max, and a random sign (+ or -)
                float angle = Random.Range(0.0f, settings.maxAngle);
                if (Random.Range(0, 2) == 0)
                {
                    angle *= -1;
                }

                direction = RotateVector(direction, angle);
            }
            else if (settings.angleType == EShotSpreadAngle.EvenlySpaced)
            {
                // shots are equally spaced
                if (shotCount > 1)
                {
                    float angleRange = settings.maxAngle * 2.0f;
                    float angleStep = angleRange / (shotCount - 1);
                    float angle = -settings.maxAngle + (angleStep * shotIndex);
                    direction = RotateVector(direction, angle);
                }
            }

            return direction.normalized;
        }

        // -------------------------------------------------------------------------------------------------

        private Vector2 RotateVector(Vector2 direction, float angleInDegrees)
        {
            Vector2 result = direction;
            if (angleInDegrees != 0.0f)
            {
                float sin = Mathf.Sin(Mathf.Deg2Rad * angleInDegrees);
                float cos = Mathf.Cos(Mathf.Deg2Rad * angleInDegrees);
                result.x = (direction.x * cos) - (direction.y * sin);
                result.y = (direction.x * sin) + (direction.y * cos);
            }

            return result;
        }

        // -------------------------------------------------------------------------------------------------
    }
}
