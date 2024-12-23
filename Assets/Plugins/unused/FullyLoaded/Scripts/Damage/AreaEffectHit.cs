using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	public struct AreaEffectHit
	{
		// -------------------------------------------------------------------------------------------------

		private DamageTarget m_target;
		private Vector3 m_position;
		private float m_distance;

		public DamageTarget target { get { return m_target; } }
		public Vector3 position { get { return m_position; } }
		public float distance { get { return m_distance; } }

		// -------------------------------------------------------------------------------------------------

		public AreaEffectHit(DamageTarget target, Vector3 position, float distance)
		{
			m_target = target;
			m_position = position;
			m_distance = distance;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
