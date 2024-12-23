using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullyLoaded;

namespace TestScene
{
	[DisallowMultipleComponent]
	public class ExampleTarget : MonoBehaviour
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private MeshRenderer m_mesh = null;
		[SerializeField] private Light m_light = null;
		[SerializeField] private Color m_flashColor = Color.red;
		[SerializeField] private float m_flashDuration = 0.25f;

		[SerializeField] private float m_bobFrequency = 0.6f;
		[SerializeField] private float m_bobHeight = 0.1f;

		[SerializeField] private float m_initialHealth = 100.0f;
		[SerializeField] private float m_minRespawnDelay = 2.0f;
		[SerializeField] private float m_maxRespawnDelay = 5.0f;
		[SerializeField] private float m_respawnDuration = 0.5f;
		[SerializeField] private GameObject m_destroyEffect = null;

		private Color m_initialColor = Color.white;
		private float m_initialLightIntensity = 0.0f;
		private float m_flashStartTime = 0.0f;
		private float m_flashEndTime = 0.0f;

		private DamageTarget m_target = null;
		private Transform m_meshTransform = null;
		private float m_health = 0.0f;
		private float m_initialYOffset = 0.0f;
		private float m_nextRespawnStart = 0.0f;
		private float m_nextRespawnEnd = 0.0f;
		private float m_initialRandomOffset = 0.0f;

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			if (m_mesh != null)
			{
				m_meshTransform = m_mesh.transform;
				m_initialYOffset = m_meshTransform.position.y;

				if (m_mesh.material != null)
				{
					m_initialColor = m_mesh.material.color;
				}
			}

			if (m_light != null)
			{
				m_initialLightIntensity = m_light.intensity;
			}

			m_target = GetComponent<DamageTarget>();
			if (m_target != null)
			{
				m_target.OnHit += OnHit;
				m_target.SetAllowDamage(false);
			}

			// schedule respawn
			m_nextRespawnStart = Time.time + Random.Range(m_minRespawnDelay, m_maxRespawnDelay);
			m_nextRespawnEnd = m_nextRespawnStart + m_respawnDuration;
			m_initialRandomOffset = Random.Range(0.0f, Mathf.PI * 2.0f);
			SetMeshScale(0.0f);
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			if (m_nextRespawnStart == 0.0f && m_nextRespawnEnd == 0.0f)
			{
				// currently alive and active
				float sin = Mathf.Sin(Mathf.PI * 2.0f * (Time.time + m_initialRandomOffset) * m_bobFrequency);

				if (m_meshTransform != null)
				{
					// bob the mesh up and down
					Vector3 pos = m_meshTransform.position;
					pos.y = m_initialYOffset + (sin * m_bobHeight);
					m_meshTransform.position = pos;
				}

				UpdateColor();
			}
			else if (Time.time >= m_nextRespawnEnd)
			{
				// respawning finished
				m_nextRespawnStart = 0.0f;
				m_nextRespawnEnd = 0.0f;
				m_health = m_initialHealth;

				m_target?.SetAllowDamage(true);
				SetMeshScale(1.0f);
			}
			else if (Time.time >= m_nextRespawnStart)
			{
				// respawning in progress
				float interp = Mathf.InverseLerp(m_nextRespawnStart, m_nextRespawnEnd, Time.time);
				SetMeshScale(interp);
			}
			else
			{
				// awaiting respawn
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void UpdateColor(bool forceInitialColor = false)
		{
			if (forceInitialColor)
			{
				if (m_mesh != null && m_mesh.material != null)
				{
					m_mesh.material.color = m_initialColor;
				}
			}
			else if (m_flashStartTime != 0.0f && m_flashEndTime != 0.0f)
			{
				if (m_mesh != null && m_mesh.material != null)
				{
					float interp = Mathf.InverseLerp(m_flashStartTime, m_flashEndTime, Time.time);
					m_mesh.material.color = Color.Lerp(m_flashColor, m_initialColor, interp);
				}

				if (Time.time >= m_flashEndTime)
				{
					m_flashStartTime = 0.0f;
					m_flashEndTime = 0.0f;
					m_mesh.material.color = m_initialColor;
				}
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void SetMeshScale(float scale)
		{
			if (m_meshTransform != null)
			{
				m_meshTransform.localScale = new Vector3(scale, scale, scale);
			}
			if (m_light != null)
			{
				m_light.intensity = m_initialLightIntensity * scale;
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

				SetMeshScale(0.0f);
				m_target?.SetAllowDamage(false);

				// spawn particle effect
				if (m_destroyEffect != null)
				{
					SpawnedEffect.Instantiate(m_destroyEffect, m_meshTransform.position, Quaternion.identity);
				}

				UpdateColor(true);
			}
			else
			{
				// damage flash
				m_flashStartTime = Time.time;
				m_flashEndTime = m_flashStartTime + m_flashDuration;
				UpdateColor();
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
