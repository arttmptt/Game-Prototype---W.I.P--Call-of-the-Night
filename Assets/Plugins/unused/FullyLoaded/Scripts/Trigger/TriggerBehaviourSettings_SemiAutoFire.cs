using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class TriggerBehaviourSettings_SemiAutoFire : TriggerBehaviourSettings
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private FrequencyValue m_maxRateOfFire;

		public FrequencyValue maxRateOfFire { get { return m_maxRateOfFire; } }

		// -------------------------------------------------------------------------------------------------
	}
}
