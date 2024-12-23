using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class FrequencyValue
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private float m_frequency = 10.0f;
		[SerializeField] private float m_interval = 0.1f;

		public float frequency { get { return Mathf.Max(0.001f, m_frequency); } }
		public float interval { get { return Mathf.Max(0.001f, m_interval); } }

		// -------------------------------------------------------------------------------------------------
	}
}
