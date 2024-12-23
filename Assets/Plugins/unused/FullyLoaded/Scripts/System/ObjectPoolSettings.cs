using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    [System.Serializable]
    public class ObjectPoolSettings
    {
        // -------------------------------------------------------------------------------------------------

        public enum EPoolLimit
        {
            NoLimit,
            FiniteSize,
        }

        // -------------------------------------------------------------------------------------------------

        public enum EPoolFullBehaviour
        {
            DisallowNewInstance,
            ReuseActiveInstance,
        }

        // -------------------------------------------------------------------------------------------------

        [SerializeField] private EPoolLimit m_limitType = EPoolLimit.NoLimit;
        [SerializeField] private EPoolFullBehaviour m_fullBehaviour = EPoolFullBehaviour.DisallowNewInstance;
        [SerializeField] private int m_limit = 0;

        public EPoolLimit limitType { get { return m_limitType; } }
        public EPoolFullBehaviour fullBehaviour { get { return m_fullBehaviour; } }
        public int limit { get { return m_limit; } }

        // -------------------------------------------------------------------------------------------------
    }
}
