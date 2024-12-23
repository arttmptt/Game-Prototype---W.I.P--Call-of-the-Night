using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class BurstConfig
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private bool m_isEnabled = false;
		[SerializeField] private bool m_useTriggerCharges = false;
		[SerializeField] private int m_burstShotCount = 1;
		[SerializeField] private float m_burstShotInterval = 1.0f;
		[SerializeField] private float m_minTimeBetweenBursts = 0.0f;

		public bool isEnabled { get { return m_isEnabled; } }
		public bool useTriggerCharges { get { return m_useTriggerCharges; } }
		public int burstShotCount { get { return Mathf.Max(1, m_burstShotCount); } }
		public float burstShotInterval { get { return Mathf.Max(0.01f, m_burstShotInterval); } }
		public float minTimeBetweenBursts { get { return Mathf.Max(0.01f, m_minTimeBetweenBursts); } }

		// -------------------------------------------------------------------------------------------------
	}
}
