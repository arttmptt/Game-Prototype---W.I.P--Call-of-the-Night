using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[CreateAssetMenu(menuName = "Fully Loaded/Projectile Definition Asset")]
	public class ProjectileDefinition : UniqueScriptableObject
	{
		// -------------------------------------------------------------------------------------------------

		// common
		[SerializeField] private EShotType m_type = EShotType.Projectile;
		[SerializeField] private float m_impactDamage = 1.0f;
		[SerializeField, AssetDropdown(allowNull = true)] private DamageType m_impactDamageType = null;

		// projectile - common
		[SerializeField] private GameObject m_projectilePrefab = null;
		[SerializeField, Layer] private int m_projectileLayer = 0;
		[SerializeField] private EPhysicsProjectileExplosionType m_explosionType = EPhysicsProjectileExplosionType.None;
		[SerializeField, Min(0.0f)] private float m_lifetime = 3.0f;
		[SerializeField, Min(0.0f)] private float m_speed = 50.0f;
		[SerializeField, Min(0.01f)] private float m_scale = 1.0f;
		[SerializeField] private bool m_randomRotation = false;

		// projectile - physics
		[SerializeField] private bool m_destroyAfterFirstImpact = true;
		[SerializeField] private bool m_allowRepeatedImpactDamage = false;
		[SerializeField] private int m_maxBounces = 0;

		// projectile - piercing
		[SerializeField] private float m_gravity = 0.0f;
		[SerializeField, Min(0.0f)] private float m_drag = 0.0f;

		// projectile - piercing / instant-hit (shared)
		[SerializeField] private string m_penetrableTag = "";
		[SerializeField] private int m_maxPenetrations = 1;
		[SerializeField] private float m_impactForce = 0.0f;

		// instant hit
		[SerializeField] private LayerMask m_hitLayers = new LayerMask();
		[SerializeField] private float m_maxDistance = 1000.0f;
		[SerializeField] private bool m_explodeOnImpact = false;

		// explosion
		[SerializeField] private float m_explosionDamageMin = 0.0f;
		[SerializeField] private float m_explosionDamageMax = 0.0f;
		[SerializeField] private float m_explosionRadius = 0.0f;
		[SerializeField] private LayerMask m_explosionOverlapLayers = new LayerMask();
		[SerializeField] private LayerMask m_explosionObstacleLayers = new LayerMask();
		[SerializeField, AssetDropdown(allowNull = true)] private DamageType m_explosionDamageType = null;

		// effects
		[SerializeField] private float m_impactEffectMinSpeed = 0.0f;
		[SerializeField] private GameObject m_impactEffect = null;
		[SerializeField] private GameObject m_explosionEffect = null;
		[SerializeField] private GameObject m_projectileTrailPrefab = null;
		[SerializeField] private GameObject m_instantHitTrailPrefab = null;

		// -------------------------------------------------------------------------------------------------

		// common
		public EShotType type { get { return m_type; } }
		public float impactDamage { get { return m_impactDamage; } }
		public DamageType impactDamageType { get { return m_impactDamageType; } }

		// projectile - common
		public GameObject projectilePrefab { get { return m_projectilePrefab; } }
		public int projectileLayer { get { return m_projectileLayer; } }
		public EPhysicsProjectileExplosionType explosionType { get { return m_explosionType; } }
		public float lifetime { get { return m_lifetime; } }
		public float speed { get { return m_speed; } }
		public float scale { get { return m_scale; } }
		public bool randomRotation { get { return m_randomRotation; } }

		// projectile - physics
		public bool destroyAfterFirstImpact { get { return m_destroyAfterFirstImpact; } }
		public bool allowRepeatedImpactDamage { get { return m_allowRepeatedImpactDamage; } }
		public int maxBounces { get { return m_maxBounces; } }

		// projectile - piercing
		public float gravity { get { return m_gravity; } }
		public float drag { get { return m_drag; } }

		// projectile - piercing / instant-hit (shared)
		public float impactForce { get { return m_impactForce; } }
		public string penetrableTag { get { return m_penetrableTag; } }
		public int maxPenetrations { get { return m_maxPenetrations; } }

		// instant hit
		public int hitLayerMask { get { return m_hitLayers.value; } }
		public float maxDistance { get { return m_maxDistance; } }
		public bool explodeOnImpact { get { return m_explodeOnImpact; } }

		// explosion
		public float explosionDamageMin { get { return m_explosionDamageMin; } }
		public float explosionDamageMax { get { return m_explosionDamageMax; } }
		public float explosionRadius { get { return m_explosionRadius; } }
		public int explosionOverlapLayers { get { return m_explosionOverlapLayers.value; } }
		public int explosionObstacleLayers { get { return m_explosionObstacleLayers.value; } }
		public DamageType explosionDamageType { get { return m_explosionDamageType; } }

		// effects
		public float impactEffectMinSpeed { get { return m_impactEffectMinSpeed; } }
		public GameObject impactEffect { get { return m_impactEffect; } }
		public GameObject explosionEffect { get { return m_explosionEffect; } }
		public GameObject projectileTrailPrefab { get { return m_projectileTrailPrefab; } }
		public GameObject instantHitTrailPrefab { get { return m_instantHitTrailPrefab; } }

		// -------------------------------------------------------------------------------------------------
	}
}
