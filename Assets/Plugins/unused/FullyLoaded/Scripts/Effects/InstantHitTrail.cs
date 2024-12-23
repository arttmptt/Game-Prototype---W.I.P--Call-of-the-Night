using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FullyLoaded
{
	[RequireComponent(typeof(LineRenderer))]
	public class InstantHitTrail : PooledObjectBase
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private float m_duration = 0.5f;
		[SerializeField] private bool m_fadeOut = true;
		[SerializeField] private UnityEvent m_onSpawned = null;
		[SerializeField] private UnityEvent m_onReleased = null;

		private Transform m_transform = null;
		private LineRenderer m_line = null;
		private float m_startTime = 0.0f;
		private float m_releaseTime = 0.0f;
		private Material m_material = null;

		// -------------------------------------------------------------------------------------------------

		public static InstantHitTrail Instantiate(GameObject prefab)
		{
			if (prefab == null)
			{
				return null;
			}

			if (prefab.GetComponent<InstantHitTrail>() == null)
			{
				Debug.LogWarning($"InstantHitTrail prefab {prefab.name} needs to have a InstantHitTrail component");
				return null;
			}

			InstantHitTrail trail = null;
			GameObject obj = ObjectPoolManager.instance.SpawnObject(prefab, Vector3.zero, Quaternion.identity);
			if (obj != null)
			{
				trail = obj.GetComponent<InstantHitTrail>();
			}

			return trail;
		}

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			m_transform = transform;

			m_line = GetComponent<LineRenderer>();
			if (m_line.material != null && m_line.material.HasProperty("_Color"))
			{
				m_material = m_line.material;
			}
		}

		// -------------------------------------------------------------------------------------------------

		public void Initialize(Vector3 startPoint, Vector3 endPoint)
		{
			m_startTime = Time.time;
			m_releaseTime = m_startTime + m_duration;

			m_transform.position = startPoint;

			m_line.positionCount = 2;
			m_line.SetPosition(0, startPoint);
			m_line.SetPosition(1, endPoint);

			if (m_onSpawned != null)
			{
				m_onSpawned.Invoke();
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			float t = Time.time;

			if (m_fadeOut && m_material != null)
			{
				if (m_startTime > 0.0f && t >= m_startTime && t < m_releaseTime)
				{
					// update the transparency
					Color col = m_material.color;
					col.a = Mathf.Lerp(1.0f, 0.0f, Mathf.InverseLerp(m_startTime, m_releaseTime, t));
					m_material.color = col;
				}
			}

			if (m_releaseTime > 0.0f && t >= m_releaseTime)
			{
				if (m_onReleased != null)
				{
					m_onReleased.Invoke();
				}

				Release();
			}
		}

		// -------------------------------------------------------------------------------------------------

		protected override void ResetInstance()
		{
			m_startTime = 0.0f;
			m_releaseTime = 0.0f;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
