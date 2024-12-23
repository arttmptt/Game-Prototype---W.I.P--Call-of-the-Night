using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class TriggerBehaviourSettings_AutomaticFire : TriggerBehaviourSettings
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private float m_spinUpTime = 0.0f;
		[SerializeField] private FrequencyValue m_rateOfFire;

		public float spinUpTime { get { return m_spinUpTime; } }
		public FrequencyValue rateOfFire { get { return m_rateOfFire; } }

		// -------------------------------------------------------------------------------------------------
	}
}
