using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public class WeaponSwitcher
    {
        // -------------------------------------------------------------------------------------------------

        private enum ESwitchState
        {
            Idle,
            SwitchingIn,
            SwitchingOut,
        }

        private WeaponEvents m_weaponEvents = null;
        private WeaponBag m_weaponBag = null;

        // weapon switch has the definitive 'current weapon'
        private WeaponDefinition m_currentWeapon = null;
        private WeaponDefinition m_pendingWeapon = null;
        public WeaponDefinition currentWeapon { get { return m_currentWeapon; } }
        public WeaponDefinition pendingWeapon { get { return m_pendingWeapon; } }

        // switch state
        private ESwitchState m_state = ESwitchState.Idle;
        private float? m_overrideSwitchInTime = null;
        private float m_startTime = 0.0f;
        private float m_endTime = 0.0f;

        public bool isBusy { get { return m_state != ESwitchState.Idle; } }

        // events
        public delegate void WeaponSwitchEvent(WeaponDefinition weapon);
        public delegate void WeaponChangedEvent(WeaponDefinition prev, WeaponDefinition next);
        public event WeaponSwitchEvent m_onWeaponSwitchStarted = null;
        public event WeaponSwitchEvent m_onWeaponSwitchFinished = null;
        public event WeaponChangedEvent m_onCurrentWeaponChanged = null;
        public event WeaponChangedEvent m_onPendingWeaponChanged = null;

        // -------------------------------------------------------------------------------------------------

        public WeaponSwitcher(WeaponEvents events, WeaponBag weaponBag)
        {
            m_weaponEvents = events;
            m_weaponBag = weaponBag;
        }

        // -------------------------------------------------------------------------------------------------

        private void SetCurrentWeapon(WeaponDefinition newCurrentWeapon)
        {
            if (newCurrentWeapon != currentWeapon)
            {
                WeaponDefinition previous = currentWeapon;
                m_currentWeapon = newCurrentWeapon;
                m_onCurrentWeaponChanged?.Invoke(previous, currentWeapon);
            }
        }

        // -------------------------------------------------------------------------------------------------

        private void SetPendingWeapon(WeaponDefinition newPendingWeapon)
        {
            if (newPendingWeapon != pendingWeapon)
            {
                WeaponDefinition previous = pendingWeapon;
                m_pendingWeapon = newPendingWeapon;
                m_onPendingWeaponChanged?.Invoke(previous, pendingWeapon);
            }
        }

        // -------------------------------------------------------------------------------------------------

        public bool SwitchToWeapon(WeaponDefinition newWeapon,
                                   float? overrideSwitchOutTime = null,
                                   float? overrideSwitchInTime = null)
        {
            if (newWeapon == pendingWeapon)
            {
                return false;
            }

            if (m_state == ESwitchState.SwitchingOut)
            {
                // if we're currently switching out, allow a chained switch to the new weapon, in which case
                // we only update the pending weapon and m_overrideSwitchInTime for an in-progress switch
                SetPendingWeapon(newWeapon);
                m_overrideSwitchInTime = overrideSwitchInTime;
                return true;
            }
            else if (m_state == ESwitchState.SwitchingIn)
            {
                // if we're currently switching in, end the switch-in early and proceed with a whole
                // new switch out + in
                OnSwitchInFinished(true);
            }
            else if (m_state == ESwitchState.Idle)
            {
                m_onWeaponSwitchStarted?.Invoke(currentWeapon);
            }

            SetPendingWeapon(newWeapon);
            m_overrideSwitchInTime = overrideSwitchInTime;

            // start the switch-out
            m_state = ESwitchState.SwitchingOut;
            m_startTime = Time.time;
            m_endTime = m_startTime;

            if (currentWeapon != null)
            {
                m_endTime += (overrideSwitchOutTime.HasValue) ?
                    Mathf.Max(0.0f, overrideSwitchOutTime.Value) :
                    currentWeapon.weaponSwitchConfig.switchOutTime;
            }

            m_weaponEvents?.TriggerWeaponSwitchOutStartedEvents(currentWeapon, m_endTime - m_startTime);

            if (m_startTime == m_endTime)
            {
                // if the switch-out time is zero the switch-in finishes immediately
                OnSwitchOutFinished();
            }

            return true;
        }

        // -------------------------------------------------------------------------------------------------

        public bool SwitchToNextWeapon(float? overrideSwitchOutTime = null,
                                       float? overrideSwitchInTime = null)
        {
            if (m_weaponBag == null)
            {
                return false;
            }

            // if we're currently idle, switch to the next weapon from the current weapon
            // otherwise chain switch from the pending weapon
            WeaponDefinition weapon = currentWeapon;
            if (m_state != ESwitchState.Idle)
            {
                weapon = pendingWeapon;
            }

            return SwitchToWeapon(m_weaponBag.GetNextWeapon(weapon, true));
        }

        // -------------------------------------------------------------------------------------------------

        public bool SwitchToPrevWeapon(float? overrideSwitchOutTime = null,
                                       float? overrideSwitchInTime = null)
        {
            if (m_weaponBag == null)
            {
                return false;
            }

            // if we're currently idle, switch to the prev weapon from the current weapon
            // otherwise chain switch from the pending weapon
            WeaponDefinition weapon = currentWeapon;
            if (m_state != ESwitchState.Idle)
            {
                weapon = pendingWeapon;
            }

            return SwitchToWeapon(m_weaponBag.GetPrevWeapon(weapon, true));
        }

        // -------------------------------------------------------------------------------------------------

        private void OnSwitchOutFinished()
        {
            m_weaponEvents?.TriggerWeaponSwitchOutFinishedEvents(currentWeapon);
            SetCurrentWeapon(pendingWeapon);

            // start the switch-in
            m_state = ESwitchState.SwitchingIn;
            m_startTime = Time.time;
            m_endTime = m_startTime;

            if (currentWeapon != null)
            {
                m_endTime += (m_overrideSwitchInTime.HasValue) ?
                    Mathf.Max(0.0f, m_overrideSwitchInTime.Value) :
                    currentWeapon.weaponSwitchConfig.switchInTime;
            }

            m_weaponEvents?.TriggerWeaponSwitchInStartedEvents(currentWeapon, m_endTime - m_startTime);

            if (m_startTime == m_endTime)
            {
                // if the switch-out time is zero the switch-out also finishes immediately
                OnSwitchInFinished(false);
            }
        }

        // -------------------------------------------------------------------------------------------------

        private void OnSwitchInFinished(bool wasCancelledEarly)
        {
            m_weaponEvents?.TriggerWeaponSwitchInFinishedEvents(currentWeapon);

            if (!wasCancelledEarly)
            {
                m_onWeaponSwitchFinished?.Invoke(currentWeapon);
            }

            // reset state
            m_state = ESwitchState.Idle;
            m_startTime = 0.0f;
            m_endTime = 0.0f;
        }

        // -------------------------------------------------------------------------------------------------

        public void UpdateWeaponSwitcher(float time, float deltaTime)
        {
            if (m_state == ESwitchState.SwitchingOut)
            {
                // check if the switch-out has completed
                if (time >= m_endTime)
                {
                    OnSwitchOutFinished();
                }
            }
            else if (m_state == ESwitchState.SwitchingIn)
            {
                // check if the switch-in has completed
                if (time >= m_endTime)
                {
                    OnSwitchInFinished(false);
                }
            }
        }

        // -------------------------------------------------------------------------------------------------
    }
}
