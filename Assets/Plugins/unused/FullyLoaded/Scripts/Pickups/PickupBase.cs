using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    [DisallowMultipleComponent]
    public abstract class PickupBase : MonoBehaviour
    {
        // -------------------------------------------------------------------------------------------------

        [SerializeField] private Transform m_visuals = null;
        [SerializeField] private EPickupSpawnMode m_spawnMode = EPickupSpawnMode.OnceOnly;
        [SerializeField] private bool m_availableAtStart = true;
        [SerializeField] private float m_respawnTime = 0.0f;

        private bool m_available = false;
        private float m_nextRespawnTime = 0.0f;

        // -------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            if (m_visuals != null)
            {
                m_visuals.gameObject.SetActive(false);
            }

            if (m_availableAtStart)
            {
                SetIsAvailable(true);
            }
        }

        // -------------------------------------------------------------------------------------------------

        protected virtual void Update()
        {
            if (m_spawnMode == EPickupSpawnMode.RespawnTimer &&
                m_respawnTime > 0.0f &&
                Time.time >= m_nextRespawnTime)
            {
                SetIsAvailable(true);
            }
        }

        // -------------------------------------------------------------------------------------------------

        public void SetIsAvailable(bool available)
        {
            if (m_available != available)
            {
                m_available = available;

                // update visuals
                if (m_visuals != null)
                {
                    m_visuals.gameObject.SetActive(available);
                }

                // set up respawn timer
                if (m_spawnMode == EPickupSpawnMode.RespawnTimer)
                {
                    m_nextRespawnTime = (available) ? 0.0f : Time.time + m_respawnTime;
                }
            }
        }

        // -------------------------------------------------------------------------------------------------

        protected abstract bool CollectItem(GameObject collector);

        // -------------------------------------------------------------------------------------------------

        #if FULLY_LOADED_2D

        private void OnTriggerEnter2D(Collider2D collider)
		{
            if (!m_available)
            {
                return;
            }

            if (collider != null && CollectItem(collider.gameObject))
            {
                SetIsAvailable(false);
            }
        }

        #else

        protected void OnTriggerEnter(Collider collider)
        {
            if (!m_available)
            {
                return;
            }

            if (collider != null && CollectItem(collider.gameObject))
            {
                SetIsAvailable(false);
            }
        }

        #endif

        // -------------------------------------------------------------------------------------------------
    }
}
