using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	public class ExampleTarget2D : MonoBehaviour
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private Transform m_targetTransform = null;
		[SerializeField] private Transform m_shadowTransform = null;

		[SerializeField] private float m_bobFrequency = 0.6f;
		[SerializeField] private float m_bobHeight = 0.1f;
		[SerializeField] private float m_minShadowScale = 0.7f;
		[SerializeField] private float m_maxShadowScale = 1.0f;

		[SerializeField] private float m_initialHealth = 100.0f;
		[SerializeField] private float m_minRespawnDelay = 2.0f;
		[SerializeField] private float m_maxRespawnDelay = 5.0f;
		[SerializeField] private float m_respawnDuration = 0.5f;
		[SerializeField] private GameObject m_destroyEffect = null;

		private DamageTarget m_target = null;
		private Transform m_transform = null;
		private float m_health = 0.0f;
		private float m_initialYPos = 0.0f;
		private float m_nextRespawnStart = 0.0f;
		private float m_nextRespawnEnd = 0.0f;
		private float m_initialRandomOffset = 0.0f;

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			m_transform = transform;
			m_target = GetComponent<DamageTarget>();
			if (m_target != null)
			{
				m_target.OnHit += OnHit;
				m_target.SetAllowDamage(false);
			}

			if (m_targetTransform != null)
			{
				m_initialYPos = m_targetTransform.position.y;
			}

			// schedule respawn
			m_nextRespawnStart = Time.time + Random.Range(m_minRespawnDelay, m_maxRespawnDelay);
			m_nextRespawnEnd = m_nextRespawnStart + m_respawnDuration;
			m_initialRandomOffset = Random.Range(0.0f, Mathf.PI * 2.0f);
			SetOverallScale(0.0f);
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			if (m_nextRespawnStart == 0.0f && m_nextRespawnEnd == 0.0f)
			{
				// currently alive and active
				float sin = Mathf.Sin(Mathf.PI * 2.0f * (Time.time + m_initialRandomOffset) * m_bobFrequency);

				if (m_targetTransform != null)
				{
					// bob the target up and down
					Vector3 pos = m_targetTransform.position;
					pos.y = m_initialYPos + (sin * m_bobHeight);
					m_targetTransform.position = pos;
				}

				if (m_shadowTransform != null)
				{
					// scale the shadow up and down
					float interp = (sin + 1.0f) * 0.5f;
					float scale = Mathf.Lerp(m_maxShadowScale, m_minShadowScale, interp);
					m_shadowTransform.localScale = new Vector3(scale, scale, 1.0f);
				}
			}
			else if (Time.time >= m_nextRespawnEnd)
			{
				// respawning finished
				m_nextRespawnStart = 0.0f;
				m_nextRespawnEnd = 0.0f;
				m_health = m_initialHealth;

				m_target?.SetAllowDamage(true);
				SetOverallScale(1.0f);
			}
			else if (Time.time >= m_nextRespawnStart)
			{
				// respawning in progress
				float interp = Mathf.InverseLerp(m_nextRespawnStart, m_nextRespawnEnd, Time.time);
				SetOverallScale(interp);
			}
			else
			{
				// awaiting respawn
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void SetOverallScale(float scale)
		{
			if (m_transform != null)
			{
				m_transform.localScale = new Vector3(scale, scale, 1.0f);
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void OnHit(Vector3 impactPoint, float damage, DamageType damageType, BaseWeaponUser source)
		{
			m_health -= damage;
			if (m_health <= 0.0f)
			{
				// target was destroyed
				m_nextRespawnStart = Time.time + Random.Range(m_minRespawnDelay, m_maxRespawnDelay);
				m_nextRespawnEnd = m_nextRespawnStart + m_respawnDuration;

				SetOverallScale(0.0f);
				m_target?.SetAllowDamage(false);

				// spawn particle effect
				if (m_destroyEffect != null)
				{
					SpawnedEffect.Instantiate(m_destroyEffect, m_targetTransform.position, Quaternion.identity);
				}
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
