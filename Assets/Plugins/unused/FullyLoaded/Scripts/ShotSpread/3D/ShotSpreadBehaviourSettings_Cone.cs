using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class ShotSpreadBehaviourSettings_Cone : ShotSpreadBehaviourSettings
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private float m_minAngle = 0.0f;
		[SerializeField] private float m_maxAngle = 0.0f;
		[SerializeField] private EShotSpreadAngle m_circumferentialAngleType = EShotSpreadAngle.Random;

		public float minAngle { get { return Mathf.Clamp(m_minAngle, 0.0f, 90.0f); } }
		public float maxAngle { get { return Mathf.Clamp(m_maxAngle, 0.0f, 90.0f); } }
		public EShotSpreadAngle circumferentialAngleType { get { return m_circumferentialAngleType; } }

		// -------------------------------------------------------------------------------------------------
	}
}
