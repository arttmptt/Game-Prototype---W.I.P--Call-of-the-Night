using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class MultiShotConfig
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private bool m_multiShotEnabled = false;
		[SerializeField] private bool m_useTriggerCharges = false;
		[SerializeField] private int m_multiShotCount = 1;

		public bool multiShotEnabled { get { return m_multiShotEnabled; } }
		public bool useTriggerCharges { get { return m_useTriggerCharges; } }
		public int multiShotCount { get { return Mathf.Max(1, m_multiShotCount); } }

		// -------------------------------------------------------------------------------------------------
	}
}
