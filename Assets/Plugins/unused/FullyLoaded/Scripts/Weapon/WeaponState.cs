using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public class WeaponState
    {
        // -------------------------------------------------------------------------------------------------

        protected bool m_triggerIsHeld = false;
        protected float m_lastTriggerPullTime = 0.0f;
        protected int m_maxTriggerCharges = 0;
        protected int m_currentTriggerCharges = 0;
        protected int m_activationCount = 0;
        protected float m_lastActivationTime = 0.0f;
        protected float m_lastEquipTime = 0.0f;
        protected float m_lastReloadTime = 0.0f;
        protected int m_shotCount = 0;
        protected float m_lastShotTime = 0.0f;
        protected int m_ammoStorageCapacity = 0;
        protected int m_ammoStorageCount = 0;

        // -------------------------------------------------------------------------------------------------

        public bool triggerIsHeld        { get { return m_triggerIsHeld; } }
        public float lastTriggerPullTime { get { return m_lastTriggerPullTime; } }
        public int maxTriggerCharges     { get { return m_maxTriggerCharges; } }
        public int currentTriggerCharges { get { return m_currentTriggerCharges; } }
        public int activationCount       { get { return m_activationCount; } }
        public float lastActivationTime  { get { return m_lastActivationTime; } }
        public float lastEquipTime       { get { return m_lastEquipTime; } }
        public float lastReloadTime      { get { return m_lastReloadTime; } }
        public int shotCount             { get { return m_shotCount; } }
        public float lastShotTime        { get { return m_lastShotTime; } }
        public int ammoStorageCapacity   { get { return m_ammoStorageCapacity; } }
        public int ammoStorageCount      { get { return m_ammoStorageCount; } }

        // -------------------------------------------------------------------------------------------------
    }
}
