using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FullyLoaded
{
	[RequireComponent(typeof(TrailRenderer))]
	public class ProjectileTrail : PooledObjectBase
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField, Min(0)] private float m_releaseDelay = 0.0f;
		[SerializeField] private UnityEvent m_onSpawned = null;
		[SerializeField] private UnityEvent m_onReleaseStarted = null;
		[SerializeField] private UnityEvent m_onReleaseFinished = null;

		private TrailRenderer m_trail = null;
		private Transform m_transform = null;
		private Transform m_defaultParent = null;
		private BaseProjectile m_projectile = null;
		private float m_releaseTime = 0.0f;

		// -------------------------------------------------------------------------------------------------

		public static ProjectileTrail Instantiate(GameObject prefab)
		{
			if (prefab == null)
			{
				return null;
			}

			if (prefab.GetComponent<ProjectileTrail>() == null)
			{
				Debug.LogWarning($"ProjectileTrail prefab {prefab.name} needs to have a ProjectileTrail component");
				return null;
			}

			ProjectileTrail trail = null;
			GameObject obj = ObjectPoolManager.instance.SpawnObject(prefab, Vector3.zero, Quaternion.identity);
			if (obj != null)
			{
				trail = obj.GetComponent<ProjectileTrail>();
			}

			return trail;
		}

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			m_transform = transform;
			m_defaultParent = m_transform.parent;

			m_trail = GetComponent<TrailRenderer>();
		}

		// -------------------------------------------------------------------------------------------------

		public void Initialize(Transform parent, BaseProjectile projectile)
		{
			m_projectile = projectile;
			if (parent != null)
			{
				m_transform.SetParent(parent);
				m_transform.localPosition = Vector3.zero;
			}

			m_trail.Clear();
			m_trail.emitting = true;

			if (m_onSpawned != null)
			{
				m_onSpawned.Invoke();
			}
		}

		// -------------------------------------------------------------------------------------------------

		public void OnProjectileReleased()
		{
			// detach from the projectile
			m_projectile = null;
			m_transform.SetParent(m_defaultParent);
			m_trail.emitting = false;

			// schedule release
			if (m_releaseDelay > 0.0f)
			{
				if (m_onReleaseStarted != null)
				{
					m_onReleaseStarted.Invoke();
				}

				m_releaseTime = Time.time + m_releaseDelay;
			}
			else
			{
				Release();
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			if (m_releaseTime > 0.0f && Time.time >= m_releaseTime)
			{
				if (m_onReleaseFinished != null)
				{
					m_onReleaseFinished.Invoke();
				}

				Release();
			}
		}

		// -------------------------------------------------------------------------------------------------

		protected override void ResetInstance()
		{
			if (m_projectile != null)
			{
				// must make the projectile aware that this trail instance can no longer be used
				m_projectile.DetachTrail();
			}

			m_projectile = null;
			m_releaseTime = 0.0f;

			m_transform.SetParent(m_defaultParent);
			m_transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		}

		// -------------------------------------------------------------------------------------------------
	}
}
