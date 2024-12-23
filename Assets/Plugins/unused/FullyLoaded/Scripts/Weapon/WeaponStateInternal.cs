using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public class WeaponStateInternal : WeaponState
    {
        // -------------------------------------------------------------------------------------------------

        public void SetTriggerIsHeld(bool value)        { m_triggerIsHeld = value; }
        public void SetLastTriggerPullTime(float value) { m_lastTriggerPullTime = value; }
        public void SetMaxTriggerCharges(int value)     { m_maxTriggerCharges = value; }
        public void SetCurrentTriggerCharges(int value) { m_currentTriggerCharges = value; }
        public void SetActivationCount(int value)       { m_activationCount = value; }
        public void SetLastActivationTime(float value)  { m_lastActivationTime = value; }
        public void SetLastEquipTime(float value)       { m_lastEquipTime = value; }
        public void SetLastReloadTime(float value)      { m_lastReloadTime = value; }
        public void SetShotCount(int value)             { m_shotCount = value; }
        public void SetLastShotTime(float value)        { m_lastShotTime = value; }
        public void SetAmmoStorageCapacity(int value)   { m_ammoStorageCapacity = value; }
        public void SetAmmoStorageCount(int value)      { m_ammoStorageCount = value; }

        // -------------------------------------------------------------------------------------------------

        public void Reset()
        {
            // called when a weapon is dropped

            SetTriggerIsHeld(false);
            SetLastTriggerPullTime(0.0f);
            SetCurrentTriggerCharges(0);
            SetActivationCount(0);
            SetLastActivationTime(0.0f);
            SetLastEquipTime(0.0f);
            SetLastReloadTime(0.0f);
            SetShotCount(0);
            SetLastShotTime(0.0f);

            // don't reset ammo storage capacity, this is initialized by the ammo config once only
            // don't reset max trigger charges, this is initialized by the trigger behaviour once only

            // weapons that have been dropped and picked up again come with a full clip
            if (ammoStorageCapacity > 0)
            {
                SetAmmoStorageCount(ammoStorageCapacity);
            }
        }

        // -------------------------------------------------------------------------------------------------

        public void OnEquipped(float equipTime, bool triggerHeldState)
        {
            SetLastEquipTime(equipTime);
            SetTriggerIsHeld(triggerHeldState);
            if (triggerHeldState)
            {
                SetLastTriggerPullTime(equipTime);
            }

            SetCurrentTriggerCharges(0);
            SetActivationCount(0);
            SetShotCount(0);
        }

        // -------------------------------------------------------------------------------------------------

        public void OnUnequipped()
        {
            SetTriggerIsHeld(false);
            SetCurrentTriggerCharges(0);
            SetActivationCount(0);
            SetShotCount(0);
        }

        // -------------------------------------------------------------------------------------------------
    }
}
