using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[DisallowMultipleComponent]
	public abstract class PooledObjectBase : MonoBehaviour
	{
		// -------------------------------------------------------------------------------------------------

		private bool m_available = true;
		private ObjectPool m_objectPool = null;

		public bool isAvailable { get { return m_available; } }

		// -------------------------------------------------------------------------------------------------

		public void SetObjectPool(ObjectPool owningPool)
		{
			if (m_objectPool == null && owningPool != null)
			{
				m_objectPool = owningPool;
			}
		}

		// -------------------------------------------------------------------------------------------------

		public void Reserve()
		{
			if (m_available)
			{
				m_available = false;
				gameObject.SetActive(true);

				m_objectPool?.Reserve(this);
			}
		}

		// -------------------------------------------------------------------------------------------------

		public void Release()
		{
			if (!m_available)
			{
				ResetInstance();

				m_available = true;
				gameObject.SetActive(false);

				m_objectPool?.Release(this);
			}
		}

		// -------------------------------------------------------------------------------------------------

		protected abstract void ResetInstance();

		// -------------------------------------------------------------------------------------------------
	}
}
