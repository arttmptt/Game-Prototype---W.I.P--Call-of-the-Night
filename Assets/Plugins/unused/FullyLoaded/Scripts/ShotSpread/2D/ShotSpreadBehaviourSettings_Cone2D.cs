using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class ShotSpreadBehaviourSettings_Cone2D : ShotSpreadBehaviourSettings2D
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private float m_maxAngle = 0.0f;
		[SerializeField] private EShotSpreadAngle m_angleType = EShotSpreadAngle.Random;

		public float maxAngle { get { return Mathf.Clamp(m_maxAngle, 0.0f, 90.0f); } }
		public EShotSpreadAngle angleType { get { return m_angleType; } }

		// -------------------------------------------------------------------------------------------------
	}
}
