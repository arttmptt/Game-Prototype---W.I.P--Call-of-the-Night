using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class ShotSpreadBehaviourSettings_Box : ShotSpreadBehaviourSettings
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private EShotSpreadAngle m_verticalAngleType = EShotSpreadAngle.Random;
		[SerializeField] private float m_minVerticalAngle = 0.0f;
		[SerializeField] private float m_maxVerticalAngle = 0.0f;
		[SerializeField] private EShotSpreadAngle m_horizontalAngleType = EShotSpreadAngle.Random;
		[SerializeField] private float m_minHorizontalAngle = 0.0f;
		[SerializeField] private float m_maxHorizontalAngle = 0.0f;

		public EShotSpreadAngle verticalAngleType { get { return m_verticalAngleType; } }
		public float minVerticalAngle { get { return Mathf.Clamp(m_minVerticalAngle, -90.0f, 90.0f); } }
		public float maxVerticalAngle { get { return Mathf.Clamp(m_maxVerticalAngle, -90.0f, 90.0f); } }
		public EShotSpreadAngle horizontalAngleType { get { return m_horizontalAngleType; } }
		public float minHorizontalAngle { get { return Mathf.Clamp(m_minHorizontalAngle, -90.0f, 90.0f); } }
		public float maxHorizontalAngle { get { return Mathf.Clamp(m_maxHorizontalAngle, -90.0f, 90.0f); } }

		// -------------------------------------------------------------------------------------------------
	}
}
