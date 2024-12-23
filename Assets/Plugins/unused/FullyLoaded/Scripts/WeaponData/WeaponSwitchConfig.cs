using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class WeaponSwitchConfig
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private float m_switchInTime = 0.0f;
		[SerializeField] private float m_switchOutTime = 0.0f;

		public float switchInTime  { get { return Mathf.Max(0.0f, m_switchInTime); } }
		public float switchOutTime { get { return Mathf.Max(0.0f, m_switchOutTime); } }

		// -------------------------------------------------------------------------------------------------
	}
}
