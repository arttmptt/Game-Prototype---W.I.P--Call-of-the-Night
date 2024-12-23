using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public abstract class TriggerBehaviourAsset : CustomBehaviourAsset, ICustomBehaviourSettingsSpawner<TriggerBehaviourSettings>
    {
        // -------------------------------------------------------------------------------------------------

        public abstract TriggerBehaviourSettings CreateBehaviourSettingsInstance();

        // -------------------------------------------------------------------------------------------------

        public abstract void InitializeWeaponState(TriggerBehaviourSettings triggerSettings, WeaponStateInternal weaponState);

        // -------------------------------------------------------------------------------------------------

        public abstract void UpdateTriggerBehaviour(float time,
                                                    float delta,
                                                    EWeaponMode weaponMode,
                                                    WeaponConfig weaponConfig,
                                                    WeaponStateInternal weaponState,
                                                    ITriggerBehaviourAmmoInterface ammoInterface,
                                                    TriggerBehaviourSettings triggerSettings,
                                                    System.Action<EWeaponMode, bool> activationFunc);

        // -------------------------------------------------------------------------------------------------
    }
}
