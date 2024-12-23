using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if FULLY_LOADED_2D
using VectorType = UnityEngine.Vector2;
using RaycastResultType = UnityEngine.RaycastHit2D;
using BaseColliderType = UnityEngine.Collider2D;
#else
using VectorType = UnityEngine.Vector3;
using RaycastResultType = UnityEngine.RaycastHit;
using BaseColliderType = UnityEngine.Collider;
#endif

namespace FullyLoaded
{
	[DisallowMultipleComponent]
	public class PiercingProjectile : BaseProjectile
	{
		// -------------------------------------------------------------------------------------------------

		private VectorType m_velocity = VectorType.zero;
		private float m_gravitySpeed = 0.0f;
		private float m_dragMultiplier = 0.0f;
		private int m_maxPenetrations = 1;
		private int m_penetrationCount = 0;
		private LayerMask m_cachedLayerMask = new LayerMask();

		private BaseColliderType m_previousHitCollider = null;
		private static RaycastResultType[] s_cachedResults = new RaycastResultType[100];

		// -------------------------------------------------------------------------------------------------

		protected override void Awake()
		{
			base.Awake();

			// ensure the default collider is disabled
			if (defaultCollider != null)
			{
				defaultCollider.enabled = false;
			}
		}

		// -------------------------------------------------------------------------------------------------

		public override void Initialize(Vector3 direction, ProjectileDefinition projectileDefinition, BaseWeaponUser source)
		{
			base.Initialize(direction, projectileDefinition, source);

			if (rigidbody != null)
			{
				rigidbody.position = transform.position;
				rigidbody.isKinematic = true;
			}

			if (projectileDefinition != null)
			{
				// cache the layer mask used for the sphere/circle-cast
				m_cachedLayerMask = LayerUtils.GetLayerCollisionMask(projectileDefinition.projectileLayer);

				m_maxPenetrations = Mathf.Max(0, projectileDefinition.maxPenetrations);
				m_gravitySpeed = Mathf.Abs(projectileDefinition.gravity);
				m_dragMultiplier = Mathf.Max(0.0f, projectileDefinition.drag);
				m_velocity = direction.normalized * projectileDefinition.speed;

				m_previousHitCollider = null;
			}
		}

		// -------------------------------------------------------------------------------------------------

		protected override void ResetInstance()
		{
			base.ResetInstance();

			m_maxPenetrations = 1;
			m_penetrationCount = 0;
			m_velocity = Vector3.zero;
			m_gravitySpeed = 0.0f;
			m_dragMultiplier = 0.0f;

			m_previousHitCollider = null;
		}

		// -------------------------------------------------------------------------------------------------

		protected virtual void FixedUpdate()
		{
			if (rigidbody != null)
			{
				// acceleration due to gravity
				m_velocity.y -= m_gravitySpeed * Time.fixedDeltaTime;

				// deceleration due to drag
				float drag = m_velocity.sqrMagnitude * m_dragMultiplier;
				if (drag * Time.fixedDeltaTime >= m_velocity.magnitude)
				{
					m_velocity = Vector3.zero;
				}
				else
				{
					m_velocity -= m_velocity.normalized * drag * Time.fixedDeltaTime;
				}

				// process hits, determine the final position and if the projectile should be released
				Vector3 pendingPos = rigidbody.position + (m_velocity * Time.fixedDeltaTime);
				Vector3 finalPos = pendingPos;
				bool needsRelease = ProcessHits(rigidbody.position, pendingPos, out finalPos);
				rigidbody.MovePosition(finalPos);
				if (needsRelease)
				{
					Release();
				}
			}
		}

		// -------------------------------------------------------------------------------------------------

		// a return value of true means that the projectile should be released/destroyed
		private bool ProcessHits(Vector3 originalPos, Vector3 pendingPos, out Vector3 finalPos)
		{
			finalPos = pendingPos;

			if (defaultCollider == null)
			{
				return false;
			}

			Vector3 direction = pendingPos - originalPos;
			float radius = defaultCollider.radius * projectileDefinition.scale;

			#if FULLY_LOADED_2D
			int hitCount = Physics2D.CircleCastNonAlloc(originalPos,
														radius,
														direction.normalized,
														s_cachedResults,
														direction.magnitude,
														m_cachedLayerMask);
			#else
			Ray ray = new Ray(originalPos, direction.normalized);
			int hitCount = Physics.SphereCastNonAlloc(ray,
				                                      radius,
													  s_cachedResults,
													  direction.magnitude,
													  m_cachedLayerMask);
			#endif

			// we are assuming results are returned in order, there is nothing to suggest otherwise
			// in the documentation (or during testing)
			RaycastResultType hit;
			for (int i = 0; i < hitCount; ++i)
			{
				hit = s_cachedResults[i];

				// if we hit one of the colliders of the source, ignore it
				if (hit.rigidbody != null && sourceObject != null && hit.rigidbody.gameObject == sourceObject)
				{
					continue;
				}

				// dont hit something we have already collided with
				if (m_previousHitCollider != null && m_previousHitCollider == hit.collider)
				{
					continue;
				}

				// trigger impact event and impact effect
				events.TriggerImpactEvents(hit.point);
				SpawnImpactEffect(0.0f, hit.point, hit.normal, true);

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

				// if we hit a DamageTarget we need to notify it
				DamageTarget target = hit.collider.GetComponentInParent<DamageTarget>();
				if (target != null && target.allowDamage)
				{
					target.RegisterDamage(hit.point, projectileDefinition.impactDamage, projectileDefinition.impactDamageType, source);
				}

				// if a penetrable tag is set and this object doesn't not have a matching tag, stop here
				// and return the position.  If no tag is set, everything is considered penetrable
				if (projectileDefinition.penetrableTag.Length > 0)
				{
					GameObject obj = (hit.rigidbody != null) ? hit.rigidbody.gameObject : hit.collider.gameObject;
					if (!obj.CompareTag(projectileDefinition.penetrableTag))
					{
						finalPos = hit.point;
						return true;
					}
				}

				// if we get this far, we penetrated, check if we have reached the maximum number of
				// penetrations allowed
				if (++m_penetrationCount >= m_maxPenetrations)
				{
					finalPos = hit.point;
					return true;
				}
				else
				{
					m_previousHitCollider = hit.collider;
				}
			}

			return false;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
