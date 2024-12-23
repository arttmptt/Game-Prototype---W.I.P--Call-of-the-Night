using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class TriggerBehaviourSettings_HoldToCharge : TriggerBehaviourSettings
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private int m_minTriggerCharges = 0;
		[SerializeField] private int m_maxTriggerCharges = 1;
		[SerializeField] private float m_timeToMinTriggerCharges = 0.0f;
		[SerializeField] private float m_timeToMaxTriggerCharges = 0.0f;
		[SerializeField] private int m_ammoUsedPerCharge = 0;
		[SerializeField] private bool m_autoFireAtMaxCharges = false;

		public int minTriggerCharges { get { return Mathf.Max(1, m_minTriggerCharges); } }
		public int maxTriggerCharges { get { return Mathf.Max(1, m_maxTriggerCharges); } }
		public float timeToMinTriggerCharges { get { return Mathf.Max(0.0f, m_timeToMinTriggerCharges); } }
		public float timeToMaxTriggerCharges { get { return Mathf.Max(0.0f, m_timeToMaxTriggerCharges); } }
		public int ammoUsedPerCharge { get { return Mathf.Max(0, m_ammoUsedPerCharge); } }
		public bool autoFireAtMaxCharges { get { return m_autoFireAtMaxCharges; } }

		// -------------------------------------------------------------------------------------------------
	}
}
