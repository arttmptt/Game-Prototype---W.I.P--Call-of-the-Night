using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[RequireComponent(typeof(LineRenderer))]
    public class ExampleLaserBeam : MonoBehaviour
    {
        // -------------------------------------------------------------------------------------------------

        [SerializeField] private WeaponDefinition m_weapon = null;
        [SerializeField] private EWeaponMode m_mode = EWeaponMode.Primary;
		[SerializeField] private float m_fadeDuration = 0.1f;
		[SerializeField] private float m_lightIntensity = 5.0f;

		private LineRenderer m_line = null;
		private Light m_light = null;

		private Transform m_transform = null;
		private WeaponUser m_weaponUser = null;
		private Material m_material = null;
		private float m_fadeStartTime = 0.0f;
		private float m_fadeEndTime = 0.0f;

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			m_transform = transform;
			m_line = GetComponent<LineRenderer>();
			m_light = GetComponent<Light>();
			m_weaponUser = GetComponentInParent<WeaponUser>();

			if (m_light != null)
			{
				m_light.intensity = 0.0f;
			}

			if (m_line != null)
			{
				m_line.positionCount = 2;
				m_line.SetPosition(0, Vector3.zero);
				m_line.SetPosition(1, Vector3.zero);

				if (m_line.material != null && m_line.material.HasProperty("_Color"))
				{
					m_material = m_line.material;
					Color col = m_material.color;
					col.a = 0.0f;
					m_material.color = col;
				}
			}

			if (m_weaponUser != null)
			{
				m_weaponUser.weaponEvents.OnActivation += OnActivation;
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void OnActivation(WeaponDefinition weapon, EWeaponMode mode, bool success)
		{
			// display the beam and immediately fade it out
			if (m_line != null && m_weapon != null && weapon == m_weapon && mode == m_mode && success)
			{
				m_fadeStartTime = Time.time;
				m_fadeEndTime = m_fadeStartTime + m_fadeDuration;
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			if (m_fadeStartTime != 0.0f && m_fadeEndTime != 0.0f)
			{
				float alpha = 0.0f;
				if (Time.time >= m_fadeEndTime)
				{
					alpha = 0.0f;
				}
				else if (Time.time >= m_fadeStartTime)
				{
					alpha = Mathf.InverseLerp(m_fadeEndTime, m_fadeStartTime, Time.time);
				}

				// set the line alpha
				if (m_material != null)
				{
					Color col = m_material.color;
					col.a = alpha;
					m_material.color = col;
				}

				// set the light intensity
				if (m_light != null)
				{
					m_light.intensity = m_lightIntensity * alpha;
				}
			}

			// update the line position
			if (m_weaponUser != null && m_line != null && m_line.positionCount == 2)
			{
				// convert the end position to local space
				Vector3 endPos = m_weaponUser.weaponAim.GetTargetPosition();
				m_line.SetPosition(1, m_transform.InverseTransformPoint(endPos));
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
