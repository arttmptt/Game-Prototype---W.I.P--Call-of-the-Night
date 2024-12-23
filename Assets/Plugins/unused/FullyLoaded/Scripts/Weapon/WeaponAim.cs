using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public class WeaponAim
    {
        // -------------------------------------------------------------------------------------------------

        private EWeaponAimMode m_aimMode = EWeaponAimMode.UseMuzzleDirection;
        private Transform m_muzzlePoint = null;
        private Transform m_targetTransform = null;
        private Vector3 m_targetPosition = Vector3.zero;

        // -------------------------------------------------------------------------------------------------

        public WeaponAim(EWeaponAimMode aimMode, Transform muzzlePoint)
        {
            m_muzzlePoint = muzzlePoint;
            SetAimMode(aimMode);
        }

        // -------------------------------------------------------------------------------------------------

        public void SetMuzzlePoint(Transform muzzlePoint)
        {
            if (muzzlePoint != null)
            {
                m_muzzlePoint = muzzlePoint;
            }
        }

        // -------------------------------------------------------------------------------------------------

        public void SetAimMode(EWeaponAimMode aimMode)
        {
            m_aimMode = aimMode;
        }

        // -------------------------------------------------------------------------------------------------

        public void SetTargetTransform(Transform target)
        {
            m_targetTransform = target;
        }

        // -------------------------------------------------------------------------------------------------

        public void SetTargetPosition(Vector2 target)
        {
            m_targetPosition = target;
        }

        // -------------------------------------------------------------------------------------------------

        public void SetTargetPosition(Vector3 target)
        {
            m_targetPosition = target;
        }

        // -------------------------------------------------------------------------------------------------

        public Vector3 GetTargetPosition()
        {
            return m_targetPosition;
        }

        // -------------------------------------------------------------------------------------------------

        public Vector3 GetSourcePosition()
        {
            Vector3 position = m_muzzlePoint.position;

            #if FULLY_LOADED_2D
            position.z = 0.0f;
            #endif

            return position;
        }

        // -------------------------------------------------------------------------------------------------

        public Vector3 GetDirection()
        {
            Vector3 direction = Vector3.up;

            switch (m_aimMode)
            {
                case EWeaponAimMode.UseMuzzleDirection:
                {
                    #if FULLY_LOADED_2D
                    direction = m_muzzlePoint.right;
                    #else
                    direction = m_muzzlePoint.forward;
                    #endif

                    break;
                }

				case EWeaponAimMode.UseTargetTransform:
                {
                    if (m_targetTransform != null)
                    {
                        direction = (m_targetTransform.position - m_muzzlePoint.position);

                        #if FULLY_LOADED_2D
                        direction.z = 0.0f;
                        #endif
                    }
                    else
                    {
                        #if FULLY_LOADED_2D
                        direction = m_muzzlePoint.right;
                        #else
                        direction = m_muzzlePoint.forward;
                        #endif
                    }
                    break;
                }

                case EWeaponAimMode.UseTargetPosition:
                {
                    direction = (m_targetPosition - m_muzzlePoint.position);

                    #if FULLY_LOADED_2D
                    direction.z = 0.0f;
                    #endif

                    break;
                }
            }

            #if !FULLY_LOADED_2D
            // don't allow an aim direction that is exactly straight up or straight down, as we
            // can't determine the 'up' or 'right' vectors in this case (for shot direction randomization)
            if (Mathf.Abs(direction.y) == 1.0f)
            {
                direction.z = 0.01f;
            }
            #endif

            return direction.normalized;
        }

        // -------------------------------------------------------------------------------------------------
    }
}
