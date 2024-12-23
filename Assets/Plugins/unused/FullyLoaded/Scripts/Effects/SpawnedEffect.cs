using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FullyLoaded
{
	public class SpawnedEffect : PooledObjectBase
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField, Min(0)] private float m_lifetime = 1.0f;
		[SerializeField] private UnityEvent m_onSpawned = null;
		[SerializeField] private UnityEvent m_onReleased = null;

		private Transform m_transform = null;
		private float m_releaseTime = 0.0f;

		// -------------------------------------------------------------------------------------------------

		public static SpawnedEffect Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			if (prefab == null)
			{
				return null;
			}

			if (prefab.GetComponent<SpawnedEffect>() == null)
			{
				Debug.LogWarning($"SpawnedEffect prefab {prefab.name} needs to have a SpawnedEffect component");
				return null;
			}

			SpawnedEffect effect = null;
			GameObject obj = ObjectPoolManager.instance.SpawnObject(prefab, Vector3.zero, rotation);
			if (obj != null)
			{
				effect = obj.GetComponent<SpawnedEffect>();
				if (effect != null)
				{
					effect.Initialize(position);
				}
			}

			return effect;
		}

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			m_transform = transform;
		}

		// -------------------------------------------------------------------------------------------------

		protected virtual void Initialize(Vector3 position)
		{
			m_releaseTime = Time.time + m_lifetime;
			m_transform.position = position;

			if (m_onSpawned != null)
			{
				m_onSpawned.Invoke();
			}
		}

		// -------------------------------------------------------------------------------------------------

		protected override void ResetInstance()
		{
			m_releaseTime = 0.0f;
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			if (m_releaseTime > 0.0f && Time.time >= m_releaseTime)
			{
				if (m_onReleased != null)
				{
					m_onReleased.Invoke();
				}

				Release();
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
