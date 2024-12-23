using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

#if FULLY_LOADED_2D
using ColliderType = UnityEngine.Collider2D;
using RaycastHitType = UnityEngine.RaycastHit2D;
#else
using ColliderType = UnityEngine.Collider;
using RaycastHitType = UnityEngine.RaycastHit;
#endif

namespace FullyLoaded
{
	public static class AreaEffect
	{
		// -------------------------------------------------------------------------------------------------

		private static ColliderType[] m_overlapResults = new ColliderType[100];
		private static RaycastHitType[] m_raycastResults = new RaycastHitType[100];
		private static List<AreaEffectHit> m_targets = new List<AreaEffectHit>(100);

		// -------------------------------------------------------------------------------------------------

		// the results collection is only valid until the next call to FindTargets
		public static ReadOnlyCollection<AreaEffectHit> FindTargets(Vector3 position,
																   float radius,
																   LayerMask overlapLayers,
																   LayerMask raycastLayers)
		{
			m_targets.Clear();

			if (radius > 0.0f)
			{
				#if FULLY_LOADED_2D
				int overlaps = Physics2D.OverlapCircleNonAlloc(position, radius, m_overlapResults, overlapLayers.value);
				#else
				int overlaps = Physics.OverlapSphereNonAlloc(position, radius, m_overlapResults, overlapLayers.value);
				#endif

				float distance = 0.0f;
				Vector3 closestPoint = Vector3.zero;
				DamageTarget target = null;
				for (int i = 0; i < overlaps; ++i)
				{
					target = m_overlapResults[i].GetComponentInParent<DamageTarget>();
					if (target != null && target.allowDamage)
					{
						closestPoint = m_overlapResults[i].ClosestPoint(position);
						distance = Vector3.Distance(closestPoint, position);

						#if FULLY_LOADED_2D
						int hits = Physics2D.RaycastNonAlloc(position,
														     (closestPoint - position).normalized,
														     m_raycastResults,
														     distance,
														     raycastLayers);
						#else
						int hits = Physics.RaycastNonAlloc(position,
														   (closestPoint - position).normalized,
														   m_raycastResults,
														   distance,
														   raycastLayers);
						#endif

						bool validObstruction = false;
						for (int j = 0; j < hits; ++j)
						{
							// ignore raycast hits at zero distance
							if (m_raycastResults[j].distance > 0.0f)
							{
								validObstruction = true;
								break;
							}
						}

						if (!validObstruction)
						{
							m_targets.Add(new AreaEffectHit(target, closestPoint, distance));
						}
					}
				}
			}

			return m_targets.AsReadOnly();
		}

		// -------------------------------------------------------------------------------------------------

		public static void DealExplosionDamage(Vector3 position, ProjectileDefinition projectile, BaseWeaponUser source)
		{
			if (projectile.explosionRadius <= 0.0f)
			{
				return;
			}

			// query hits
			ReadOnlyCollection<AreaEffectHit> hits =
				AreaEffect.FindTargets(position, projectile.explosionRadius, projectile.explosionOverlapLayers, projectile.explosionObstacleLayers);

			// calculate and deal damage
			float oneOverRadius = 1.0f / projectile.explosionRadius;
			for (int i = 0; i < hits.Count; ++i)
			{
				if (hits[i].target != null)
				{
					float interp = Mathf.Clamp01(hits[i].distance * oneOverRadius);
					float damage = Mathf.Lerp(projectile.explosionDamageMax, projectile.explosionDamageMin, interp);
					hits[i].target.RegisterDamage(hits[i].position, damage, projectile.explosionDamageType, source);
				}
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
