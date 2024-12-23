using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	public class ObjectPool
	{
		// -------------------------------------------------------------------------------------------------

		private List<PooledObjectBase> m_usedObjects = new List<PooledObjectBase>();
		private List<PooledObjectBase> m_availableObjects = new List<PooledObjectBase>();

		public int usedCount { get { return m_usedObjects.Count; } }
		public int availableCount { get { return m_availableObjects.Count; } }

		// -------------------------------------------------------------------------------------------------

		public ObjectPool(int initialCapacity = 0)
		{
			if (initialCapacity > 0)
			{
				m_usedObjects.Capacity = initialCapacity;
				m_availableObjects.Capacity = initialCapacity;
			}
		}

		// -------------------------------------------------------------------------------------------------

		public void AddInstance(PooledObjectBase instance)
		{
			if (instance != null)
			{
				instance.SetObjectPool(this);
				m_availableObjects.Add(instance);
			}
		}

		// -------------------------------------------------------------------------------------------------

		public void Reserve(PooledObjectBase instance)
		{
			m_availableObjects.Remove(instance);
			m_usedObjects.Add(instance);
		}

		// -------------------------------------------------------------------------------------------------

		public void Release(PooledObjectBase instance)
		{
			m_usedObjects.Remove(instance);
			m_availableObjects.Add(instance);
		}

		// -------------------------------------------------------------------------------------------------

		public PooledObjectBase FindFreeInstance()
		{
			if (m_availableObjects.Count > 0)
			{
				return m_availableObjects[0];
			}

			return null;
		}

		// -------------------------------------------------------------------------------------------------

		public PooledObjectBase FindOldestActiveInstance()
		{
			if (m_usedObjects.Count > 0)
			{
				return m_usedObjects[0];
			}

			return null;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
