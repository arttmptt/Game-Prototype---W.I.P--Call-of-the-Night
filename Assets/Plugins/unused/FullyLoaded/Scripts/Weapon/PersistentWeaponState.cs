using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public class PersistentWeaponState
    {
        // -------------------------------------------------------------------------------------------------

        public struct WeaponStatePair
        {
            public WeaponStateInternal primary;
            public WeaponStateInternal secondary;
        }

        private Dictionary<WeaponDefinition, WeaponStatePair> m_weaponState = new Dictionary<WeaponDefinition, WeaponStatePair>();

        // -------------------------------------------------------------------------------------------------

        public bool AddWeapon(WeaponDefinition weaponDef)
        {
            if (weaponDef == null || m_weaponState.ContainsKey(weaponDef))
            {
                return false;
            }

            // lambda to initialize the weapon state
            System.Action<WeaponConfig, WeaponStateInternal> InitWeaponState =
                (WeaponConfig weaponConfig, WeaponStateInternal weaponState) =>
                {
                    // allow the TriggerBehaviour a chance to initialize the weapon state
                    if (weaponConfig.triggerConfig != null)
                        {
                            TriggerBehaviourConfig triggerConfig = weaponConfig.triggerConfig;
                            if (triggerConfig.behaviour != null)
                            {
                                triggerConfig.behaviour.InitializeWeaponState(triggerConfig.settings, weaponState);
                            }
                        }

                    // allow the AmmoConfig a chance to initialize the weapon state
                    weaponConfig.ammoConfig.InitializeWeaponState(weaponState);
                };

            // create and initialize a new weapon state for primary/secondary fire modes
            WeaponStatePair pair = new WeaponStatePair();
            if (weaponDef.primaryFire != null)
            {
                pair.primary = new WeaponStateInternal();
                InitWeaponState(weaponDef.primaryFire, pair.primary);
            }
            if (weaponDef.secondaryFire != null && weaponDef.secondaryFireEnabled)
            {
                pair.secondary = new WeaponStateInternal();
                InitWeaponState(weaponDef.secondaryFire, pair.secondary);
            }
            m_weaponState.Add(weaponDef, pair);
            return true;
        }

        // -------------------------------------------------------------------------------------------------

        public bool RemoveWeapon(WeaponDefinition weaponDef)
        {
            if (weaponDef == null || !m_weaponState.ContainsKey(weaponDef))
            {
                return false;
            }

            m_weaponState.Remove(weaponDef);
            return true;
        }

        // -------------------------------------------------------------------------------------------------

        public void ResetWeaponState(WeaponDefinition weaponDef)
        {
            WeaponStatePair pair;
            if (weaponDef != null && m_weaponState.TryGetValue(weaponDef, out pair))
            {
                pair.primary?.Reset();
                pair.secondary?.Reset();
            }
        }

        // -------------------------------------------------------------------------------------------------

        public void ResetWeaponState(WeaponDefinition weaponDef, EWeaponMode weaponMode)
        {
            WeaponStatePair pair;
            if (weaponDef != null && m_weaponState.TryGetValue(weaponDef, out pair))
            {
                WeaponStateInternal state = (weaponMode == EWeaponMode.Primary) ? pair.primary : pair.secondary;
                state?.Reset();
            }
        }

        // -------------------------------------------------------------------------------------------------

        public WeaponStatePair GetState(WeaponDefinition weaponDef)
        {
            WeaponStatePair pair = new WeaponStatePair();
            if (weaponDef != null)
            {
                m_weaponState.TryGetValue(weaponDef, out pair);
            }

            return pair;
        }

        // -------------------------------------------------------------------------------------------------
    }
}
