using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	#if FULLY_LOADED_DEV
	[CreateAssetMenu(menuName = "Fully Loaded/Trigger Behaviour Asset/SemiAuto")]
	#endif
	public class TriggerBehaviourAsset_SemiAutoFire : TriggerBehaviourAsset
	{
		// -------------------------------------------------------------------------------------------------

		public override TriggerBehaviourSettings CreateBehaviourSettingsInstance()
		{
			return new TriggerBehaviourSettings_SemiAutoFire();
		}

		// -------------------------------------------------------------------------------------------------

		public override void InitializeWeaponState(TriggerBehaviourSettings triggerSettings, WeaponStateInternal weaponState)
		{
			if (weaponState != null)
			{
				weaponState.SetMaxTriggerCharges(0);
				weaponState.SetCurrentTriggerCharges(0);
			}
		}

		// -------------------------------------------------------------------------------------------------

		public override void UpdateTriggerBehaviour(float time,
													float delta,
													EWeaponMode weaponMode,
													WeaponConfig weaponConfig,
													WeaponStateInternal weaponState,
													ITriggerBehaviourAmmoInterface ammoInterface,
													TriggerBehaviourSettings triggerSettings,
													System.Action<EWeaponMode, bool> activationFunc)
		{
			if (!weaponState.triggerIsHeld)
			{
				return;
			}

			var settings = triggerSettings as TriggerBehaviourSettings_SemiAutoFire;
			if (settings == null)
			{
				Debug.LogWarning("failed to cast TriggerBehaviourSettings to correct type");
				return;
			}

			// if burst mode is active, fireInterval cannot be less than the burst duration
			float fireInterval = settings.maxRateOfFire.interval;
			BurstConfig burstConfig = weaponConfig.burstConfig;
			if (burstConfig.isEnabled && burstConfig.burstShotCount > 1)
			{
				float burstDuration = burstConfig.burstShotInterval * (burstConfig.burstShotCount - 1);
				fireInterval = Mathf.Max(fireInterval, burstDuration + burstConfig.minTimeBetweenBursts);
			}

			if (weaponState.lastActivationTime == 0.0f || weaponState.lastTriggerPullTime > weaponState.lastActivationTime)
			{
				// we have released and pulled the trigger since the last activation
				// and we aren't trying to exceed the rate of fire
				if (time >= weaponState.lastActivationTime + fireInterval)
				{
					activationFunc(weaponMode, true);
				}
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
