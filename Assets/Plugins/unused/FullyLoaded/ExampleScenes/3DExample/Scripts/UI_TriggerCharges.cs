using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public class UI_TriggerCharges : MonoBehaviour
    {
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private EWeaponMode m_mode = EWeaponMode.Primary;
        [SerializeField] private Transform m_grid = null;
        [SerializeField] private Transform m_chargeObject = null;
		[SerializeField] private float m_chargeDuration = 0.4f;

		private WeaponUser m_weaponUser = null;
		private List<GameObject> m_charges = new List<GameObject>();

		private int m_previousCharges = 0;
		private float m_resetTime = 0.0f;

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			// find the player's weapon user
			GameObject obj = GameObject.FindGameObjectWithTag("Player");
			if (obj != null)
			{
				m_weaponUser = obj.GetComponent<WeaponUser>();

				// register for weapon activation events, this is to catch the final charge count
				// since trigger charges are cleared immediately after activation
				m_weaponUser.weaponEvents.OnActivation += OnWeaponActivation;
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void OnWeaponActivation(WeaponDefinition weapon, EWeaponMode mode, bool success)
		{
			if (m_weaponUser != null && mode == m_mode)
			{
				UpdateCharges();
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void Update()
		{
			if (m_weaponUser != null)
			{
				UpdateCharges();
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void UpdateCharges()
		{
			if (m_weaponUser != null)
			{
				int charges = 0;

				WeaponState state = m_weaponUser.GetWeaponState(m_mode);
				if (state != null && state.maxTriggerCharges > 0)
				{
					charges = state.currentTriggerCharges;
				}

				if (m_previousCharges > 0 && charges == 0)
				{
					m_resetTime = Time.time + m_chargeDuration;
				}
				m_previousCharges = charges;

				// setting the charge count back to zero has a small delay, otherwise
				// you never see the final charge if it triggers an automatic fire
				if (charges > 0 || (m_resetTime != 0.0f && Time.time >= m_resetTime))
				{
					UpdateUIGrid(charges);
				}
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void UpdateUIGrid(int charges)
		{
			// need to instantiate more charges and add them to the list?
			if (charges > m_charges.Count)
			{
				for (int i = 0; i < (charges - m_charges.Count); ++i)
				{
					GameObject newObj = Instantiate(m_chargeObject.gameObject);
					newObj.transform.SetParent(m_grid);
					m_charges.Add(newObj);
				}
			}

			// set enabled/disabled state of all charges
			for (int i = 0; i < m_charges.Count; ++i)
			{
				if (m_charges[i] != null)
				{
					m_charges[i].SetActive(i < charges);
				}
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
