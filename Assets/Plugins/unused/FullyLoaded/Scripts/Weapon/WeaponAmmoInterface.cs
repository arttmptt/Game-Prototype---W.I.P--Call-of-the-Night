using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public class WeaponAmmoInterface : ITriggerBehaviourAmmoInterface
    {
        // -------------------------------------------------------------------------------------------------

        private ScheduledAction<bool> m_reloadAction = new ScheduledAction<bool>();
        private bool m_blockingReload = false;

        private IAmmoSource m_ammoSource = null;
        private WeaponDefinition m_weapon = null;
        private EWeaponMode m_weaponMode = EWeaponMode.Primary;
        private WeaponConfig m_weaponConfig = null;
        private WeaponStateInternal m_weaponState = null;
        private WeaponEvents m_weaponEvents = null;

        private AmmoType m_ammoType = null;
        private bool m_hasInfiniteAmmo = false;

        private bool m_useClip = false;
        private int m_clipSize = 0;

        // -------------------------------------------------------------------------------------------------

        public WeaponAmmoInterface(WeaponEvents weaponEvents)
        {
            m_weaponEvents = weaponEvents;

            m_reloadAction.SetCallback(OnReloadFinished);
        }

        // -------------------------------------------------------------------------------------------------

        public void Reset(IAmmoSource ammoSource,
                          WeaponDefinition weapon,
                          EWeaponMode weaponMode,
                          WeaponConfig weaponConfig,
                          WeaponStateInternal weaponState)
        {
            m_reloadAction.Reset();
            m_blockingReload = false;

            m_ammoSource = ammoSource;
            m_weapon = weapon;
            m_weaponMode = weaponMode;
            m_weaponConfig = weaponConfig;
            m_weaponState = weaponState;

            m_ammoType = null;
            m_hasInfiniteAmmo = false;
            m_useClip = false;
            m_clipSize = 0;

            if (m_weaponConfig != null)
            {
                m_useClip = m_weaponConfig.ammoConfig.hasReloadableClip;
                m_clipSize = m_weaponConfig.ammoConfig.clipSize;

                // cache ammo-type and whether we have infinite ammo or not
                m_ammoType = m_weaponConfig.ammoConfig.ammoType;
                m_hasInfiniteAmmo = m_weaponConfig.ammoConfig.isInfinite;
                if (m_ammoSource != null && m_ammoType != null && m_ammoSource.HasInfiniteAmmo(m_ammoType))
                {
                    m_hasInfiniteAmmo = true;
                }
            }
        }

        // -------------------------------------------------------------------------------------------------

        public void UpdateAmmoInterface(float time, float delta)
        {
            m_reloadAction.Update(time);
        }

        // -------------------------------------------------------------------------------------------------

        public bool IsReloadInProgress()
        {
            return m_reloadAction.isActive;
        }

        // -------------------------------------------------------------------------------------------------

        public bool IsBlockingReloadInProgress()
        {
            return m_reloadAction.isActive && m_blockingReload;
        }

        // -------------------------------------------------------------------------------------------------

        public bool StartReload(EReloadType reloadType, bool blockingReload)
        {
            // already a reload in progress
            if (m_reloadAction.isActive)
            {
                return false;
            }

            // reload is either not allowed, not necessary, or there is no ammo to reload
            if (!IsReloadAllowed(reloadType) || !NeedsReload() || !HasAmmoAvailableToReload())
            {
                return false;
            }

            float reloadTime = m_weaponConfig.ammoConfig.reloadTime;

            // trigger reload started event
            m_weaponEvents?.TriggerReloadStartedEvents(m_weapon, m_weaponMode, reloadType, reloadTime);

            // schedule the reload
            m_blockingReload = blockingReload;
            m_reloadAction.Schedule(Time.time, reloadTime, blockingReload);
            return true;
        }

        // -------------------------------------------------------------------------------------------------

        private bool OnReloadFinished(bool blockingReload)
        {
            Reload();

            m_blockingReload = false;
            m_reloadAction.Reset();

            m_weaponState.SetLastReloadTime(Time.time);

            // trigger reload finished event
            m_weaponEvents?.TriggerReloadFinishedEvents(m_weapon, m_weaponMode);

            return true;
        }

        // -------------------------------------------------------------------------------------------------

        public bool CheckHasSufficientAmmo(int ammoNeeded)
        {
            if (ammoNeeded <= 0)
            {
                return true;
            }

            if (m_useClip)
            {
                // do we have enough rounds in the weapon's clip
                return (m_weaponState.ammoStorageCount >= ammoNeeded);
            }
            else
            {
                if (m_hasInfiniteAmmo)
                {
                    return true;
                }
                else if (m_ammoType != null && m_ammoSource != null)
                {
                    return (m_ammoSource.GetAmmoCount(m_ammoType) >= ammoNeeded);
                }
            }

            return false;
        }

        // -------------------------------------------------------------------------------------------------

        public bool ConsumeAmmo(int ammoNeeded)
        {
            if (ammoNeeded <= 0)
            {
                return true;
            }

            if (m_useClip)
            {
                // check we have enough rounds in the weapon's clip, and remove them if so
                if (m_weaponState.ammoStorageCount >= ammoNeeded)
                {
                    m_weaponState.SetAmmoStorageCount(m_weaponState.ammoStorageCount - ammoNeeded);
                    return true;
                }
            }
            else
            {
                // if we have infinite ammo the result is always successful
                // otherwise we need to check we have sufficient ammo to consume first
                if (m_hasInfiniteAmmo)
                {
                    return true;
                }
                else if (m_ammoType != null && m_ammoSource != null)
                {
                    if (m_ammoSource.GetAmmoCount(m_ammoType) >= ammoNeeded)
                    {
                        m_ammoSource.RemoveAmmo(m_ammoType, ammoNeeded);
                        return true;
                    }
                }
            }

            return false;
        }

        // -------------------------------------------------------------------------------------------------

        public bool IsReloadAllowed(EReloadType reloadType)
        {
            if (m_weaponConfig != null)
            {
                if (reloadType == EReloadType.Manual)
                {
                    return m_weaponConfig.ammoConfig.allowManualReload;
                }
                else if (reloadType == EReloadType.Automatic)
                {
                    return m_weaponConfig.ammoConfig.allowAutoReload;
                }
            }

            return false;
        }

        // -------------------------------------------------------------------------------------------------

        public bool NeedsReload()
        {
            return (m_useClip && m_weaponState.ammoStorageCount < m_weaponState.ammoStorageCapacity);
        }

        // -------------------------------------------------------------------------------------------------

        public bool HasAmmoAvailableToReload()
        {
            if (m_hasInfiniteAmmo)
            {
                return true;
            }

            return (m_ammoSource != null && m_ammoSource.GetAmmoCount(m_ammoType) > 0);
        }

        // -------------------------------------------------------------------------------------------------

        public void Reload()
        {
            // clear any trigger charges
            m_weaponState.SetCurrentTriggerCharges(0);

            // if we have infinite ammo, just refil the clip to max
            // otherwise attempt take as many rounds as needed to fill the clip from the ammo source
            if (m_hasInfiniteAmmo)
            {
                m_weaponState.SetAmmoStorageCount(m_weaponState.ammoStorageCapacity);
            }
            else if (m_ammoType != null && m_ammoSource != null)
            {
                int requiredRounds = Mathf.Max(0, m_weaponState.ammoStorageCapacity - m_weaponState.ammoStorageCount);
                int acquiredRounds = m_ammoSource.RemoveAmmo(m_ammoType, requiredRounds);
                m_weaponState.SetAmmoStorageCount(Mathf.Min(m_weaponState.ammoStorageCount + acquiredRounds, m_weaponState.ammoStorageCapacity));
            }
        }

        // -------------------------------------------------------------------------------------------------

        public void Cancel()
        {
            m_blockingReload = false;
            m_reloadAction.Reset();
        }

        // -------------------------------------------------------------------------------------------------
    }
}
