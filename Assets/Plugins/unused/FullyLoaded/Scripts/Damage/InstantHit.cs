using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if FULLY_LOADED_2D
using RaycastResultType = UnityEngine.RaycastHit2D;
#else
using RaycastResultType = UnityEngine.RaycastHit;
#endif

namespace FullyLoaded
{
    public static class InstantHit
    {
        // -------------------------------------------------------------------------------------------------

        #if !FULLY_LOADED_2D
        // comparer for sorting results by distance
        private class HitComparer : IComparer<RaycastResultType>
        {
            public int Compare(RaycastResultType left, RaycastResultType right)
            {
                return left.distance.CompareTo(right.distance);
            }
        }

        private static HitComparer s_hitComparer = null;
        #endif

        private static RaycastResultType[] s_cachedResults = new RaycastResultType[100];

        // -------------------------------------------------------------------------------------------------

        // returns true if a final position was set (something non-penetrable stopped the raycast)
        public static bool ProcessInstantHit(Vector3 origin,
                                             Vector3 direction,
                                             ProjectileDefinition projectileDefinition,
                                             BaseWeaponUser source,
                                             out Vector3 finalPosition)
        {
            #if FULLY_LOADED_2D
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(projectileDefinition.hitLayerMask);
            int hitCount = Physics2D.Raycast(origin,
                                             direction,
                                             filter,
                                             s_cachedResults,
                                             projectileDefinition.maxDistance);
            #else
            Ray ray = new Ray(origin, direction);
            int hitCount = Physics.RaycastNonAlloc(ray,
                                                   s_cachedResults,
                                                   projectileDefinition.maxDistance,
                                                   projectileDefinition.hitLayerMask);

            // sort hits by ascending distance
            if (s_hitComparer == null)
            {
                s_hitComparer = new HitComparer();
            }
            System.Array.Sort(s_cachedResults, 0, hitCount, s_hitComparer);
            #endif

            // process hits
            int penetrationCount = 0;
            RaycastResultType hit;
            for (int i = 0; i < hitCount; ++i)
            {
                hit = s_cachedResults[i];

                #if FULLY_LOADED_2D
                // don't hit colliders we start inside of
                if (hit.fraction == 0.0f)
                {
                    continue;
                }
                #endif

                // ignore colliders on the source
                if (source != null &&
                    hit.rigidbody != null &&
                    hit.rigidbody.gameObject == source.gameObject)
                {
                    continue;
                }
                
                // spawn impact effect
                if (projectileDefinition.impactEffect != null)
                {
                    Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    SpawnedEffect.Instantiate(projectileDefinition.impactEffect, hit.point, rot);
                }

                // apply force to the thing we hit
                if (projectileDefinition.impactForce > 0.0f && hit.rigidbody != null)
                {
                    #if FULLY_LOADED_2D
					hit.rigidbody.AddForceAtPosition(-hit.normal * projectileDefinition.impactForce,
                                                     hit.point,
                                                     ForceMode2D.Impulse);
                    #else
                    hit.rigidbody.AddForceAtPosition(-hit.normal * projectileDefinition.impactForce,
                                                     hit.point,
                                                     ForceMode.Impulse);
                    #endif
                }

                // find a damage target and damage it
                DamageTarget target = hit.collider.GetComponentInParent<DamageTarget>();
                if (target != null)
                {
                    target.RegisterDamage(hit.point,
                                          projectileDefinition.impactDamage,
                                          projectileDefinition.impactDamageType,
                                          source);
                }

                // if a penetrable tag is set and this object doesn't not have a matching tag, stop here
                // and return the position.  If no tag is set, everything is considered penetrable
                if (projectileDefinition.penetrableTag.Length > 0)
                {
                    GameObject obj = (hit.rigidbody != null) ? hit.rigidbody.gameObject : hit.collider.gameObject;
                    if (!obj.CompareTag(projectileDefinition.penetrableTag))
                    {
                        finalPosition = hit.point;
                        return true;
                    }
                }

                // if we get this far, we penetrated, check if we have reached the maximum number of
                // penetrations allowed, and if so stop there
                if (++penetrationCount >= projectileDefinition.maxPenetrations)
                {
                    finalPosition = hit.point;
                    return true;
                }
            }

            // no unpenetrable end-point was hit
            finalPosition = origin + (direction * projectileDefinition.maxDistance);
            return false;
        }

        // -------------------------------------------------------------------------------------------------
    }
}
