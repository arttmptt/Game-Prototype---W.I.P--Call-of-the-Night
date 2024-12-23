using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	#if FULLY_LOADED_DEV
	[CreateAssetMenu(menuName = "Fully Loaded/Trigger Behaviour Asset/HoldToCharge")]
	#endif
	public class TriggerBehaviourAsset_HoldToCharge : TriggerBehaviourAsset
	{
		// -------------------------------------------------------------------------------------------------

		public override TriggerBehaviourSettings CreateBehaviourSettingsInstance()
		{
			return new TriggerBehaviourSettings_HoldToCharge();
		}

		// -------------------------------------------------------------------------------------------------

		public override void InitializeWeaponState(TriggerBehaviourSettings triggerSettings, WeaponStateInternal weaponState)
		{
			var settings = triggerSettings as TriggerBehaviourSettings_HoldToCharge;
			if (settings == null)
			{
				Debug.LogWarning("failed to cast TriggerBehaviourSettings to correct type");
				return;
			}

			if (weaponState != null)
			{
				weaponState.SetMaxTriggerCharges(settings.maxTriggerCharges);
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
			var settings = triggerSettings as TriggerBehaviourSettings_HoldToCharge;
			if (settings == null)
			{
				Debug.LogWarning("failed to cast TriggerBehaviourSettings to correct type");
				return;
			}

			// ignore trigger pulls that occurred before the last activation/weapon switch/reload
			if (weaponState.lastTriggerPullTime <= weaponState.lastActivationTime ||
				weaponState.lastTriggerPullTime <= weaponState.lastEquipTime ||
				weaponState.lastTriggerPullTime <= weaponState.lastReloadTime)
			{
				return;
			}

			if (weaponState.triggerIsHeld)
			{
				// determine the current charge level while the trigger is held
				int charges = 0;

				// how much ammo is needed for activation
				int activationAmmoNeeded = weaponConfig.ammoConfig.ammoUsedPerActivation;
				int minChargesAmmoNeeded = settings.ammoUsedPerCharge * settings.minTriggerCharges;

				// calculate how many trigger charges we should have by this time
				float chargeDuration = time - weaponState.lastTriggerPullTime;
				if (chargeDuration >= settings.timeToMaxTriggerCharges)
				{
					charges = settings.maxTriggerCharges;
				}
				else if (chargeDuration >= settings.timeToMinTriggerCharges)
				{
					// once we hit this point an activation is guaranteed, unless we switch weapons before firing
					// check we have sufficient ammo for an activation, if not we trigger the activation
					// immediately (which will fail due to the ammo check)
					// this prevents us from continuing to charge up while having insufficient ammo to do so
					if (!ammoInterface.CheckHasSufficientAmmo(activationAmmoNeeded) ||
						(weaponState.currentTriggerCharges == 0 && !ammoInterface.CheckHasSufficientAmmo(activationAmmoNeeded + minChargesAmmoNeeded)))
					{
						// clear any charges that were acquired and activate early
						weaponState.SetCurrentTriggerCharges(0);
						activationFunc(weaponMode, false);
						return;
					}

					charges = settings.minTriggerCharges;
					chargeDuration = Mathf.Max(0.0f, chargeDuration - settings.timeToMinTriggerCharges, 0.0f);
					float fullyChargedDuration = Mathf.Max(0.0f, settings.timeToMaxTriggerCharges - settings.timeToMinTriggerCharges);
					float durationPerCharge = fullyChargedDuration / (settings.maxTriggerCharges - settings.minTriggerCharges);
					charges += Mathf.FloorToInt(chargeDuration / durationPerCharge);
				}

				// spend ammo for new trigger charges acquired since last update
				// must always leave enough ammo for activation if necessary
				// if we run out, we activate immediately with whatever charges we have
				bool outOfAmmo = false;
				if (charges > weaponState.currentTriggerCharges)
				{
					int chargesDelta = charges - weaponState.currentTriggerCharges;
					charges = weaponState.currentTriggerCharges;
					for (int i = 0; i < chargesDelta; ++i)
					{
						if (ammoInterface.CheckHasSufficientAmmo(settings.ammoUsedPerCharge + activationAmmoNeeded))
						{
							// spend ammo and accumulate the charge
							ammoInterface.ConsumeAmmo(settings.ammoUsedPerCharge);
							charges++;
						}
						else
						{
							// insuffient ammo, activate immediately with whatever charges we can afford
							outOfAmmo = true;
							break;
						}
					}
				}

				weaponState.SetCurrentTriggerCharges(charges);

				// do we need to auto-activate at max charges, despite not having released the trigger yet?
				if (outOfAmmo || (settings.autoFireAtMaxCharges && charges == settings.maxTriggerCharges))
				{
					activationFunc(weaponMode, true);

					// clear any charges that were acquired
					weaponState.SetCurrentTriggerCharges(0);
				}
			}
			else
			{
				// new trigger release
				activationFunc(weaponMode, (weaponState.currentTriggerCharges > 0));

				// clear any charges that were acquired
				weaponState.SetCurrentTriggerCharges(0);
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
