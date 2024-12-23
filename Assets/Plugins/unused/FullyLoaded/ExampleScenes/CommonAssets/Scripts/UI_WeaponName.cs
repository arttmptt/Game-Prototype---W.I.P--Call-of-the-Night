using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FullyLoaded
{
    public class UI_WeaponName : MonoBehaviour
    {
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private WeaponUser m_weaponUser = null;
		[SerializeField] private float m_displayDuration = 1.0f;
		[SerializeField] private float m_fadeDuration = 1.0f;

		private Text m_text = null;
		private WeaponDefinition m_currentWeapon = null;
		private float m_fadeStartTime = 0.0f;
		private float m_fadeEndTime = 0.0f;

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			// initialize the text
			m_text = GetComponentInChildren<Text>();
			if (m_text != null)
			{
				m_text.text = "";
			}

			// register for weapon switch events
			if (m_weaponUser != null)
			{
				m_currentWeapon = m_weaponUser.currentWeapon;
				m_weaponUser.weaponEvents.OnWeaponSwitchInStarted += OnWeaponSwitchIn;
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void SetAlpha(float alpha)
		{
			if (m_text != null)
			{
				Color col = m_text.color;
				col.a = alpha;
				m_text.color = col;
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void OnWeaponSwitchIn(WeaponDefinition weapon, float duration)
		{
			if (m_text != null && m_currentWeapon != weapon)
			{
				if (weapon != null)
				{
					m_text.text = weapon.weaponName;
					m_fadeStartTime = Time.time + m_displayDuration;
					m_fadeEndTime = m_fadeStartTime + m_fadeDuration;
					SetAlpha(1.0f);
				}
				else
				{
					m_text.text = "";
				}
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			if (m_fadeStartTime > 0.0f && Time.time >= m_fadeStartTime)
			{
				if (m_fadeEndTime > 0.0f && Time.time >= m_fadeEndTime)
				{
					SetAlpha(0.0f);
					m_fadeStartTime = 0.0f;
					m_fadeEndTime = 0.0f;
				}
				else
				{
					SetAlpha(Mathf.InverseLerp(m_fadeEndTime, m_fadeStartTime, Time.time));
				}
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
