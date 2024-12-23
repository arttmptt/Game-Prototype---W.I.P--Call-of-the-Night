using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FullyLoaded
{
    [System.Serializable]
    public class WeaponEvents
    {
        // -------------------------------------------------------------------------------------------------

        public delegate void WeaponEvent_Trigger(WeaponDefinition weapon,
                                                 EWeaponMode mode,
                                                 bool triggerState);

        public delegate void WeaponEvent_Activation(WeaponDefinition weapon,
                                                    EWeaponMode mode,
                                                    bool success);

        public delegate void WeaponEvent_Shot(WeaponDefinition weapon,
                                              EWeaponMode mode,
                                              bool success);

        public delegate void WeaponEvent_ReloadStarted(WeaponDefinition weapon,
                                                       EWeaponMode mode,
                                                       EReloadType reloadType,
                                                       float reloadDuration);

        public delegate void WeaponEvent_ReloadFinished(WeaponDefinition weapon,
                                                        EWeaponMode mode);

        public delegate void WeaponEvent_WeaponSwitchStarted(WeaponDefinition weapon,
                                                             float duration);

        public delegate void WeaponEvent_WeaponSwitchFinished(WeaponDefinition weapon);

        public delegate void WeaponEvent_WeaponOwnership(WeaponDefinition weapon,
                                                         bool ownership);

        // -------------------------------------------------------------------------------------------------

        // c# events
        public event WeaponEvent_Trigger OnTriggerChanged = null;
        public event WeaponEvent_Activation OnActivation = null;
        public event WeaponEvent_Shot OnShotsFired = null;
        public event WeaponEvent_ReloadStarted OnReloadStarted = null;
        public event WeaponEvent_ReloadFinished OnReloadFinished = null;
        public event WeaponEvent_WeaponSwitchStarted OnWeaponSwitchOutStarted = null;
        public event WeaponEvent_WeaponSwitchFinished OnWeaponSwitchOutFinished = null;
        public event WeaponEvent_WeaponSwitchStarted OnWeaponSwitchInStarted = null;
        public event WeaponEvent_WeaponSwitchFinished OnWeaponSwitchInFinished = null;
        public event WeaponEvent_WeaponOwnership OnOwnershipChanged = null;

        // -------------------------------------------------------------------------------------------------

        // unity events
        [System.Serializable]
        public class UnityEvents
        {
            [SerializeField] public UnityEvent<bool> OnTriggerChanged = null;
            [SerializeField] public UnityEvent<bool> OnActivation = null;
            [SerializeField] public UnityEvent<bool> OnShotsFired = null;
            [SerializeField] public UnityEvent<float> OnReloadStarted = null;
            [SerializeField] public UnityEvent OnReloadFinished = null;
            [SerializeField] public UnityEvent<float> OnWeaponSwitchOutStarted = null;
            [SerializeField] public UnityEvent OnWeaponSwitchOutFinished = null;
            [SerializeField] public UnityEvent<float> OnWeaponSwitchInStarted = null;
            [SerializeField] public UnityEvent OnWeaponSwitchInFinished = null;
            [SerializeField] public UnityEvent<WeaponDefinition, bool> OnOwnershipChanged = null;
        }

        [SerializeField] private UnityEvents m_unityEvents = new UnityEvents();

        // -------------------------------------------------------------------------------------------------

        public void TriggerTriggerChangedEvents(WeaponDefinition weapon,
                                                EWeaponMode mode,
                                                bool triggerState)
        {
            if (OnTriggerChanged != null)
            {
                OnTriggerChanged(weapon, mode, triggerState);
            }
            m_unityEvents.OnTriggerChanged?.Invoke(triggerState);
        }

        // -------------------------------------------------------------------------------------------------

        public void TriggerActivationEvents(WeaponDefinition weapon,
                                            EWeaponMode mode,
                                            bool success)
        {
            if (OnActivation != null)
            {
                OnActivation(weapon, mode, success);
            }
            m_unityEvents.OnActivation?.Invoke(success);
        }

        // -------------------------------------------------------------------------------------------------

        public void TriggerShotEvents(WeaponDefinition weapon,
                                      EWeaponMode mode,
                                      bool success)
        {
            if (OnShotsFired != null)
            {
                OnShotsFired(weapon, mode, success);
            }
            m_unityEvents.OnShotsFired?.Invoke(success);
        }

        // -------------------------------------------------------------------------------------------------

        public void TriggerReloadStartedEvents(WeaponDefinition weapon,
                                               EWeaponMode mode,
                                               EReloadType reloadType,
                                               float reloadDuration)
        {
            if (OnReloadStarted != null)
            {
                OnReloadStarted(weapon, mode, reloadType, reloadDuration);
            }
            m_unityEvents.OnReloadStarted?.Invoke(reloadDuration);
        }

        // -------------------------------------------------------------------------------------------------

        public void TriggerReloadFinishedEvents(WeaponDefinition weapon,
                                               EWeaponMode mode)
        {
            if (OnReloadFinished != null)
            {
                OnReloadFinished(weapon, mode);
            }
            m_unityEvents.OnReloadFinished?.Invoke();
        }

        // -------------------------------------------------------------------------------------------------

        public void TriggerWeaponSwitchOutStartedEvents(WeaponDefinition weapon,
                                                        float duration)
        {
            if (OnWeaponSwitchOutStarted != null)
            {
                OnWeaponSwitchOutStarted(weapon, duration);
            }
            m_unityEvents.OnWeaponSwitchOutStarted?.Invoke(duration);
        }

        // -------------------------------------------------------------------------------------------------

        public void TriggerWeaponSwitchOutFinishedEvents(WeaponDefinition weapon)
        {
            if (OnWeaponSwitchOutFinished != null)
            {
                OnWeaponSwitchOutFinished(weapon);
            }
            m_unityEvents.OnWeaponSwitchOutFinished?.Invoke();
        }

        // -------------------------------------------------------------------------------------------------

        public void TriggerWeaponSwitchInStartedEvents(WeaponDefinition weapon,
                                                       float duration)
        {
            if (OnWeaponSwitchInStarted != null)
            {
                OnWeaponSwitchInStarted(weapon, duration);
            }
            m_unityEvents.OnWeaponSwitchInStarted?.Invoke(duration);
        }

        // -------------------------------------------------------------------------------------------------

        public void TriggerWeaponSwitchInFinishedEvents(WeaponDefinition weapon)
        {
            if (OnWeaponSwitchInFinished != null)
            {
                OnWeaponSwitchInFinished(weapon);
            }
            m_unityEvents.OnWeaponSwitchInFinished?.Invoke();
        }

        // -------------------------------------------------------------------------------------------------

        public void TriggerOwnershipChangedEvents(WeaponDefinition weapon, bool ownership)
        {
            if (OnOwnershipChanged != null)
            {
                OnOwnershipChanged(weapon, ownership);
            }
            m_unityEvents.OnOwnershipChanged?.Invoke(weapon, ownership);
        }

        // -------------------------------------------------------------------------------------------------
    }
}
