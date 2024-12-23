using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if FULLY_LOADED_2D
using BaseColliderType = UnityEngine.Collider2D;
#else
using BaseColliderType = UnityEngine.Collider;
#endif

namespace FullyLoaded
{
	[DisallowMultipleComponent]
	public class PhysicsProjectile : BaseProjectile
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private bool m_ignoreInitialOverlaps = false;

		private List<BaseColliderType> m_initialColliders = new List<BaseColliderType>();
		private static BaseColliderType[] m_cachedColliders = new BaseColliderType[10];

		private int m_bounceCount = 0;
		private bool m_haveDealtImpactDamage = false;

		// -------------------------------------------------------------------------------------------------

		public override void Initialize(Vector3 direction, ProjectileDefinition projectileDefinition, BaseWeaponUser source)
		{
			base.Initialize(direction, projectileDefinition, source);

			if (rigidbody != null)
			{
				rigidbody.position = transform.position;
				rigidbody.linearVelocity = direction * projectileDefinition.speed;
			}

			if (m_ignoreInitialOverlaps)
			{
				FindInitiallyOverlappingColliders();
				SetIgnoreInitialColliders(true);
			}
		}

		// -------------------------------------------------------------------------------------------------

		protected override void ResetInstance()
		{
			base.ResetInstance();

			m_bounceCount = 0;
			m_haveDealtImpactDamage = false;

			if (m_ignoreInitialOverlaps)
			{
				SetIgnoreInitialColliders(false);
				m_initialColliders.Clear();
			}

			if (rigidbody != null)
			{
				rigidbody.linearVelocity = Vector3.zero;
			}
		}

		// -------------------------------------------------------------------------------------------------

		protected override void OnLifetimeExpired()
		{
			base.OnLifetimeExpired();

			// if set to explode after fuse, explode
			if (projectileDefinition.explosionType == EPhysicsProjectileExplosionType.ExplodeAfterFuse)
			{
				Explode();
			}
		}

		// -------------------------------------------------------------------------------------------------

		// do an overlap test to find any colliders the projectile is already inside, these are later ignored
		// for collision in order to prevent the projectile from hitting the thing that spawned it
		//
		// note: this only works for the default circle/sphere collider on the projectile
		//       any additional colliders could potentially collide with the spawner
		protected void FindInitiallyOverlappingColliders()
		{
			m_initialColliders.Clear();

			if (defaultCollider != null)
			{
				// which layers we should care about for the overlap test
				int overlapLayerMask = LayerUtils.GetLayerCollisionMask(projectileDefinition.projectileLayer);

				#if FULLY_LOADED_2D

				// must modify the collider centre/radius by the local scale
				Vector2 position = (Vector2)(defaultCollider.transform.position) + (defaultCollider.offset * projectileDefinition.scale);
				float radius = defaultCollider.radius * projectileDefinition.scale;
				int overlaps = Physics2D.OverlapCircleNonAlloc(position, radius, m_cachedColliders, overlapLayerMask);
				
				#else

				// must modify the collider centre/radius by the local scale
				Vector3 position = defaultCollider.transform.position + (defaultCollider.center * projectileDefinition.scale);
				float radius = defaultCollider.radius * projectileDefinition.scale;
				int overlaps = Physics.OverlapSphereNonAlloc(position, radius, m_cachedColliders, overlapLayerMask);

				#endif

				if (overlaps > 0)
				{
					for (int i = 0; i < overlaps; ++i)
					{
						m_initialColliders.Add(m_cachedColliders[i]);
					}
				}
			}
		}

		// -------------------------------------------------------------------------------------------------

		protected void SetIgnoreInitialColliders(bool ignoreCollisions)
		{
			for (int i = 0; i < m_initialColliders.Count; ++i)
			{
				if (m_initialColliders[i] != null)
				{
					#if FULLY_LOADED_2D
					Physics2D.IgnoreCollision(defaultCollider, m_cachedColliders[i], ignoreCollisions);
					#else
					Physics.IgnoreCollision(defaultCollider, m_cachedColliders[i], ignoreCollisions);
					#endif
				}
			}
		}

		// -------------------------------------------------------------------------------------------------

#if FULLY_LOADED_2D

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (isAvailable)
			{
				// it's possible we hit multiple things this frame, only the first one counts
				// in which case future collisions will not work as the projectile has already
				// been released to the object pool
				return;
			}

			if (collision.rigidbody != null &&
				sourceObject != null &&
				collision.rigidbody.gameObject == sourceObject)
			{
				// we hit one of the colliders of the source, ignore it
				return;
			}

			if (collision.collider != null)
			{
				Vector2 normal = Vector2.zero;
				for (int i = 0; i < collision.contactCount; ++i)
				{
					normal += collision.GetContact(i).normal;
				}
				normal.Normalize();

				HandleCollision(collision.gameObject, normal, collision.relativeVelocity);
			}
		}

#else

		private void OnCollisionEnter(Collision collision)
		{
			if (isAvailable)
			{
				// it's possible we hit multiple things this frame, only the first one counts
				// in which case future collisions will not work as the projectile has already
				// been released to the object pool
				return;
			}

			if (collision.rigidbody != null &&
				sourceObject != null &&
				collision.rigidbody.gameObject == sourceObject)
			{
				// we hit one of the colliders of the source, ignore it
				return;
			}

			if (collision.collider != null)
			{
				Vector3 normal = Vector3.zero;
				for (int i = 0; i < collision.contactCount; ++i)
				{
					normal += collision.GetContact(i).normal;
				}
				normal.Normalize();

				HandleCollision(collision.gameObject, normal, collision.relativeVelocity);
			}
		}

#endif

		// -------------------------------------------------------------------------------------------------

		protected virtual void HandleCollision(GameObject other, Vector3 normal, Vector3 relativeVel)
		{
			events.TriggerImpactEvents(transform.position);

			SpawnImpactEffect(relativeVel.magnitude, transform.position, normal);

			// if we hit a DamageTarget we need to notify it
			DamageTarget target = other.GetComponentInParent<DamageTarget>();
			if (target != null &&
				target.allowDamage &&
				(projectileDefinition.allowRepeatedImpactDamage || !m_haveDealtImpactDamage))
			{
				target.RegisterDamage(transform.position, projectileDefinition.impactDamage, projectileDefinition.impactDamageType, source);
				m_haveDealtImpactDamage = true;
			}

			// trigger any necessary explosion
			if (projectileDefinition.explosionType == EPhysicsProjectileExplosionType.ExplodeOnImpact)
			{
				Explode();
			}

			// so long as explosion type is not fuse-based or destroy on hit is true, any collision
			// will destroy the projectile
			if (projectileDefinition.explosionType == EPhysicsProjectileExplosionType.ExplodeOnImpact ||
				(projectileDefinition.explosionType == EPhysicsProjectileExplosionType.None && projectileDefinition.destroyAfterFirstImpact))
			{
				Release();
			}
			else
			{
				// enforce max bounces
				if (m_bounceCount < projectileDefinition.maxBounces)
				{
					m_bounceCount++;
				}
				else if (projectileDefinition.destroyAfterFirstImpact ||
					     projectileDefinition.explosionType == EPhysicsProjectileExplosionType.ExplodeOnImpact)
				{
					// destroy once bounce count is exceeded
					Release();
				}
				else
				{
					rigidbody.linearVelocity = Vector3.zero;
				}
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
