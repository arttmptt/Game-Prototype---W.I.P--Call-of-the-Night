using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if FULLY_LOADED_2D
using BaseColliderType = UnityEngine.Collider2D;
using DefaultColliderType = UnityEngine.CircleCollider2D;
using RigidbodyType = UnityEngine.Rigidbody2D;
#else
using BaseColliderType = UnityEngine.Collider;
using DefaultColliderType = UnityEngine.SphereCollider;
using RigidbodyType = UnityEngine.Rigidbody;
#endif

namespace FullyLoaded
{
	[DisallowMultipleComponent]
	public abstract class BaseProjectile : PooledObjectBase
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private ProjectileEvents m_events = new ProjectileEvents();

		private Transform m_transform = null;
		private RigidbodyType m_rigidbody = null;
		private BaseWeaponUser m_source = null;
		private GameObject m_sourceObject = null;

		private DefaultColliderType m_defaultCollider = null;
		private List<BaseColliderType> m_childColliders = new List<BaseColliderType>();

		private ProjectileDefinition m_projectileDefinition = null;
		private ProjectileTrail m_trail = null;
		private float m_expiryTime = 0.0f;

		protected new Transform transform { get { return m_transform; } }
		protected new RigidbodyType rigidbody { get { return m_rigidbody; } }
		protected DefaultColliderType defaultCollider { get { return m_defaultCollider; } }
		protected ProjectileEvents events { get { return m_events; } }
		protected BaseWeaponUser source { get { return m_source; } }
		protected GameObject sourceObject { get { return m_sourceObject; } }
		protected ProjectileDefinition projectileDefinition { get { return m_projectileDefinition; } }

		// -------------------------------------------------------------------------------------------------

		protected virtual void Awake()
		{
			m_transform = GetComponent<Transform>();
			m_rigidbody = GetComponent<RigidbodyType>();
			m_defaultCollider = GetComponent<DefaultColliderType>();

			// cache all child objects so that we can set them all to be in the same layer
			GetComponentsInChildren<BaseColliderType>(true, m_childColliders);
		}

		// -------------------------------------------------------------------------------------------------

		public virtual void Initialize(Vector3 direction, ProjectileDefinition projectileDefinition, BaseWeaponUser source)
		{
			// set all child colliders to be in the projectile layer
			gameObject.layer = projectileDefinition.projectileLayer;
			for (int i = 0; i < m_childColliders.Count; ++i)
			{
				m_childColliders[i].gameObject.layer = projectileDefinition.projectileLayer;
			}

			// cache the source (the WeaponUser that fired this)
			m_source = source;
			m_sourceObject = (source != null) ? source.gameObject : null;

			m_projectileDefinition = projectileDefinition;
			if (m_projectileDefinition != null)
			{
				// if projectile lifetime is set, set an expiry time
				if (m_projectileDefinition.lifetime > 0.0f)
				{
					m_expiryTime = Time.time + m_projectileDefinition.lifetime;
				}

				// set scale
				m_transform.localScale = new Vector3(projectileDefinition.scale, projectileDefinition.scale, projectileDefinition.scale);

				// either randomize the rotation, or set it to point towards the direction
				#if FULLY_LOADED_2D
				m_transform.rotation = (projectileDefinition.randomRotation) ?
					Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)) :
					Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.up, direction));
				#else
				m_transform.rotation = (projectileDefinition.randomRotation) ?
					Random.rotationUniform :
					Quaternion.LookRotation(direction);
				#endif

				// instantiate the projectile trail for this projectile
				m_trail = ProjectileTrail.Instantiate(projectileDefinition.projectileTrailPrefab);
				if (m_trail != null)
				{
					m_trail.Initialize(m_transform, this);
				}
			}

			m_events.TriggerSpawnedEvents(m_transform.position, direction);
		}

		// -------------------------------------------------------------------------------------------------

		protected virtual void OnLifetimeExpired()
		{
		}

		// -------------------------------------------------------------------------------------------------

		protected virtual void Update()
		{
			if (m_expiryTime > 0.0f && Time.time >= m_expiryTime)
			{
				OnLifetimeExpired();
				Release();
			}
		}

		// -------------------------------------------------------------------------------------------------

		protected override void ResetInstance()
		{
			m_events.TriggerDestroyedEvents(m_transform.position);

			m_trail?.OnProjectileReleased();
			m_trail = null;

			m_expiryTime = 0.0f;
			m_projectileDefinition = null;
			m_source = null;
			m_sourceObject = null;
		}

		// -------------------------------------------------------------------------------------------------

		public void DetachTrail()
		{
			m_trail = null;
		}

		// -------------------------------------------------------------------------------------------------

		protected void SpawnImpactEffect(float impactSpeed, Vector3 position, Vector3 normal, bool alwaysSpawn = false)
		{
			if (alwaysSpawn ||
				(projectileDefinition.impactEffect != null &&
				impactSpeed >= projectileDefinition.impactEffectMinSpeed))
			{
				Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);
				SpawnedEffect.Instantiate(projectileDefinition.impactEffect, position, rot);
			}
		}

		// -------------------------------------------------------------------------------------------------

		protected virtual void Explode()
		{
			AreaEffect.DealExplosionDamage(m_transform.position, projectileDefinition, m_source);

			// spawn explosion effect
			if (projectileDefinition.explosionEffect != null)
			{
				SpawnedEffect.Instantiate(projectileDefinition.explosionEffect, m_transform.position, Quaternion.identity);
			}

			m_events.TriggerExplodedEvents(m_transform.position);
		}

		// -------------------------------------------------------------------------------------------------
	}
}
