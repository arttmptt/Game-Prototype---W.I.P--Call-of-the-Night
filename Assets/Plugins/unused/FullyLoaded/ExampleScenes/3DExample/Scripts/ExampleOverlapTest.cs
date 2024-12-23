using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class ExampleOverlapTest
	{
		// -------------------------------------------------------------------------------------------------

		private SphereCollider m_groundCollider = null;
		private SphereCollider m_obstacleCollider = null;
		private LayerMask m_layers = new LayerMask();
		private RaycastHit[] m_groundCastResults = new RaycastHit[10];

		// -------------------------------------------------------------------------------------------------

		public ExampleOverlapTest(SphereCollider groundCollider,
								  SphereCollider obstacleCollider,
								  LayerMask layers,
								  Transform colliderMoveTransform)
		{
			m_groundCollider = groundCollider;
			m_obstacleCollider = obstacleCollider;
			m_layers = layers;
		}

		// -------------------------------------------------------------------------------------------------

		public bool CheckForOverlap(Vector3 position)
		{
			if (m_obstacleCollider != null)
			{
				return Physics.CheckSphere(position + m_obstacleCollider.center,
										   m_obstacleCollider.radius,
										   m_layers.value);
			}

			return false;
		}

		// -------------------------------------------------------------------------------------------------

		public bool FindGround(Vector3 position, float distance, out Vector3 normal, out float correction)
		{
			normal = Vector3.zero;
			correction = 0.0f;
			if (m_groundCollider == null)
			{
				return false;
			}

			// perform a sphere cast downwards
			int hits = Physics.SphereCastNonAlloc(position + m_groundCollider.center, m_groundCollider.radius, Vector3.down, m_groundCastResults, distance, m_layers.value);

			// find the closest hit
			int closestHit = -1;
			float closestHitDistance = float.MaxValue;
			for (int i = 0; i < hits; ++i)
			{
				RaycastHit hit = m_groundCastResults[i];
				if (hit.distance == 0.0f && hit.point == Vector3.zero && hit.normal == Vector3.up)
				{
					// we're inside something
					continue;
				}
				if (hit.distance < closestHitDistance)
				{
					closestHit = i;
					closestHitDistance = hit.distance;
				}
			}

			// if no valid hits, there is no ground
			if (closestHit == -1)
			{
				return false;
			}

			normal = m_groundCastResults[closestHit].normal;
			correction = m_groundCollider.radius - m_groundCastResults[closestHit].distance;
			return true;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
