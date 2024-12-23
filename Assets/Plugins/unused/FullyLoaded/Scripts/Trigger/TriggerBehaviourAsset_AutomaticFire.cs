using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	#if FULLY_LOADED_DEV
	[CreateAssetMenu(menuName = "Fully Loaded/Trigger Behaviour Asset/Automatic")]
	#endif
	public class TriggerBehaviourAsset_AutomaticFire : TriggerBehaviourAsset
	{
		// -------------------------------------------------------------------------------------------------

		public override TriggerBehaviourSettings CreateBehaviourSettingsInstance()
		{
			return new TriggerBehaviourSettings_AutomaticFire();
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

			var settings = triggerSettings as TriggerBehaviourSettings_AutomaticFire;
			if (settings == null)
			{
				Debug.LogWarning("failed to cast TriggerBehaviourSettings to correct type");
				return;
			}

			// if burst mode is active, fireInterval cannot be less than the burst duration
			float fireInterval = settings.rateOfFire.interval;
			BurstConfig burstConfig = weaponConfig.burstConfig;
			if (burstConfig.isEnabled && burstConfig.burstShotCount > 1)
			{
				float burstDuration = burstConfig.burstShotInterval * (burstConfig.burstShotCount - 1);
				fireInterval = Mathf.Max(fireInterval, burstDuration + burstConfig.minTimeBetweenBursts);
			}

			// cache weapon state timestamps
			float lastActivationTime = weaponState.lastActivationTime;
			float lastTriggerTime = weaponState.lastTriggerPullTime;
			float lastReloadTime = weaponState.lastReloadTime;

			// have we fired a first shot for this trigger pull?
			bool firedFirstShot =
				lastActivationTime != 0.0f &&
				lastTriggerTime != 0.0f &&
				lastActivationTime >= lastTriggerTime;

			// have we fired a first shot since the last reload (if any)
			bool firedSinceReload =
				lastActivationTime != 0.0f &&
				lastActivationTime > weaponState.lastReloadTime;

			// time at which firing first becomes possible
			float triggerStartTime = Mathf.Max(lastTriggerTime, lastReloadTime) + settings.spinUpTime;

			float nextShotTime = 0.0f;
			if (firedFirstShot && firedSinceReload)
			{
				nextShotTime = lastActivationTime + fireInterval;
			}
			else
			{
				nextShotTime = triggerStartTime;
				if (nextShotTime < lastActivationTime + fireInterval)
				{
					nextShotTime = lastActivationTime + fireInterval;
				}
			}

			if (time >= nextShotTime)
			{
				float t = nextShotTime;
				while (t <= time)
				{
					activationFunc(weaponMode, true);
					t += fireInterval;
				}
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
