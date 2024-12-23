using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EPoolLimit = FullyLoaded.ObjectPoolSettings.EPoolLimit;
using EPoolFullBehaviour = FullyLoaded.ObjectPoolSettings.EPoolFullBehaviour;

namespace FullyLoaded
{
    [DisallowMultipleComponent]
    public class ObjectPoolManager : MonoBehaviour
    {
        // -------------------------------------------------------------------------------------------------

        [System.Serializable]
        public class ObjectPoolSettingsOverride
        {
            [SerializeField] private GameObject m_prefab = null;
            [SerializeField] private ObjectPoolSettings m_settings = new ObjectPoolSettings();

            public GameObject prefab { get { return m_prefab; } }
            public ObjectPoolSettings settings { get { return m_settings; } }
        }

        // -------------------------------------------------------------------------------------------------

        [SerializeField] private ObjectPoolSettings m_defaultPoolSettings = new ObjectPoolSettings();
        [SerializeField] private List<ObjectPoolSettingsOverride> m_settingsOverrides = new List<ObjectPoolSettingsOverride>();

        private Dictionary<GameObject, ObjectPoolSettings> m_settingsLookup = new Dictionary<GameObject, ObjectPoolSettings>();
        private Dictionary<GameObject, ObjectPool> m_objectPools = new Dictionary<GameObject, ObjectPool>();
        private Dictionary<GameObject, Transform> m_poolTransforms = new Dictionary<GameObject, Transform>();
        private Transform m_transform = null;

        private readonly int kDefaultPoolSize = 16;

        // -------------------------------------------------------------------------------------------------

        // singleton access
        private static ObjectPoolManager m_instance = null;
        public static ObjectPoolManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    // if there is no instance, either find one in the scene or create a new one
                    m_instance = FindObjectOfType<ObjectPoolManager>();
                    if (m_instance == null)
                    {
                        GameObject obj = new GameObject("ObjectPoolManager");
                        m_instance = obj.AddComponent<ObjectPoolManager>();
                    }
                }

                return m_instance;
            }
        }

        // -------------------------------------------------------------------------------------------------

        private void Awake()
        {
            // destroy duplicate copies
            if (m_instance == null)
            {
                m_instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // initialization
            Initialize();
        }

        // -------------------------------------------------------------------------------------------------

        private void Initialize()
        {
            m_transform = transform;

            // build the lookup dictionary for settings overrides
            for (int i = 0; i < m_settingsOverrides.Count; ++i)
            {
                GameObject prefab = m_settingsOverrides[i].prefab;

                if (prefab == null || prefab.GetComponent<PooledObjectBase>() == null)
                {
                    Debug.LogWarning($"invalid prefab set in SettingsOverrides");
                }
                else if (m_settingsLookup.ContainsKey(prefab))
                {
                    Debug.LogWarning($"duplicate entry set in SettingsOverrides");
                }
                else
                {
                    m_settingsLookup.Add(prefab, m_settingsOverrides[i].settings);
                }
            }
        }

        // -------------------------------------------------------------------------------------------------

        private ObjectPoolSettings FindPoolSettings(GameObject prefab)
        {
            if (prefab != null && m_settingsLookup.TryGetValue(prefab, out ObjectPoolSettings settings))
            {
                return settings;
            }

            return m_defaultPoolSettings;
        }

        // -------------------------------------------------------------------------------------------------

        public GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
            {
                return null;
            }
            if (prefab.GetComponent<PooledObjectBase>() == null)
            {
                Debug.LogWarning($"{prefab.name} must have a component that inherits from PooledObjectBase");
                return null;
            }

            // look up the pool settings
            ObjectPoolSettings settings = FindPoolSettings(prefab);

            if (!m_objectPools.ContainsKey(prefab))
            {
                // create dictionary entries
                GameObject parentObject = new GameObject(prefab.name);
                parentObject.transform.SetParent(m_transform);
                m_poolTransforms.Add(prefab, parentObject.transform);

                int poolLimit = (settings.limitType == EPoolLimit.FiniteSize) ? settings.limit : kDefaultPoolSize;
                m_objectPools.Add(prefab, new ObjectPool(poolLimit));
            }

            // look for an available object in the pool
            ObjectPool pool = m_objectPools[prefab];
            PooledObjectBase obj = pool.FindFreeInstance();
            if (obj != null && obj.isAvailable)
            {
                // found an available object, reuse it
                obj.Reserve();
                obj.gameObject.transform.position = position;
                obj.gameObject.transform.rotation = rotation;
                return obj.gameObject;
            }

            // are we over the pool limit?
            if (settings.limitType == EPoolLimit.FiniteSize && pool.usedCount >= settings.limit)
            {
                if (settings.fullBehaviour == EPoolFullBehaviour.DisallowNewInstance)
                {
                    // new instances are not allowed
                    return null;
                }
                else if (settings.fullBehaviour == EPoolFullBehaviour.ReuseActiveInstance)
                {
                    // re-use the oldest in-use instance
                    obj = pool.FindOldestActiveInstance();
                    if (obj != null)
                    {
                        obj.Release();
                        obj.Reserve();
                        obj.gameObject.transform.position = position;
                        obj.gameObject.transform.rotation = rotation;
                        return obj.gameObject;
                    }
                }
            }
            else
            {
                // instantiate a fresh object and add it to the pool
                GameObject spawnedObj = GameObject.Instantiate(prefab, position, rotation, m_poolTransforms[prefab]);
                PooledObjectBase spawnedComponent = spawnedObj.GetComponent<PooledObjectBase>();
                if (spawnedComponent != null)
                {
                    pool.AddInstance(spawnedComponent);
                    spawnedComponent.Reserve();
                    return spawnedObj;
                }
            }

            return null;
        }

        // -------------------------------------------------------------------------------------------------
    }
}
