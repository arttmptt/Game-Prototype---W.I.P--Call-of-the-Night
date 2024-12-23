using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FullyLoaded
{
    [System.Serializable]
    public class ProjectileEvents
    {
        // -------------------------------------------------------------------------------------------------

        // unity events
        [System.Serializable]
        public class UnityEvents
        {
            [SerializeField] public UnityEvent<Vector3, Vector3> OnSpawned = null;
            [SerializeField] public UnityEvent<Vector3> OnImpact = null;
            [SerializeField] public UnityEvent<Vector3> OnExploded = null;
            [SerializeField] public UnityEvent<Vector3> OnDestroyed = null;
        }

        [SerializeField] private UnityEvents m_unityEvents = new UnityEvents();

        // -------------------------------------------------------------------------------------------------

        public void TriggerSpawnedEvents(Vector3 position, Vector3 direction)
        {
            m_unityEvents.OnSpawned?.Invoke(position, direction);
        }

        // -------------------------------------------------------------------------------------------------

        public void TriggerImpactEvents(Vector3 position)
        {
            m_unityEvents.OnImpact?.Invoke(position);
        }

        // -------------------------------------------------------------------------------------------------

        public void TriggerExplodedEvents(Vector3 position)
        {
            m_unityEvents.OnExploded?.Invoke(position);
        }

        // -------------------------------------------------------------------------------------------------

        public void TriggerDestroyedEvents(Vector3 position)
        {
            m_unityEvents.OnDestroyed?.Invoke(position);
        }

        // -------------------------------------------------------------------------------------------------
    }
}
