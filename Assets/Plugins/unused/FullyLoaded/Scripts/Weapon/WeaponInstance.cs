using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	public class WeaponInstance
	{
		// -------------------------------------------------------------------------------------------------

		private struct WeaponInstanceConfig
		{
			public EWeaponMode weaponMode;
			public WeaponConfig weaponConfig;
			public WeaponAmmoInterface ammoInterface;
			public WeaponStateInternal weaponState;
		}

		private BaseWeaponUser m_owner = null;
		private WeaponAim m_weaponAim = null;
		private IAmmoSource m_ammoSource = null;
		private WeaponEvents m_weaponEvents = null;
		private WeaponDefinition m_weapon = null;
		private WeaponInstanceConfig[] m_configs = new WeaponInstanceConfig[2];
		private bool m_equipped = false;

		private ScheduledAction<EWeaponMode> m_burstActionPrimary = new ScheduledAction<EWeaponMode>();
		private ScheduledAction<EWeaponMode> m_burstActionSecondary = new ScheduledAction<EWeaponMode>();

		// to prevent allocations each frame when passing the Activation delegate to TriggerBehaviour
		private System.Action<EWeaponMode, bool> m_cachedActivateDelegate = null;

		// -------------------------------------------------------------------------------------------------

		public WeaponInstance(BaseWeaponUser owner,
			                  WeaponAim weaponAim,
							  IAmmoSource ammoSource,
							  WeaponEvents weaponEvents)
		{
			m_owner = owner;
			m_weaponEvents = weaponEvents;
			m_cachedActivateDelegate = Activate;

			m_burstActionPrimary.SetCallback(FireShot);
			m_burstActionSecondary.SetCallback(FireShot);

			if (weaponEvents != null)
			{
				m_configs[0].ammoInterface = new WeaponAmmoInterface(m_weaponEvents);
				m_configs[1].ammoInterface = new WeaponAmmoInterface(m_weaponEvents);
			}

			Reset(weaponAim, ammoSource, null, null, null);
		}

		// -------------------------------------------------------------------------------------------------

		public void Reset(WeaponAim weaponAim,
						  IAmmoSource ammoSource,
						  WeaponDefinition weaponDefinition,
						  WeaponStateInternal primaryWeaponState,
						  WeaponStateInternal secondaryWeaponState)
		{
			m_weapon = weaponDefinition;
			m_weaponAim = weaponAim;
			m_ammoSource = ammoSource;

			m_burstActionPrimary.Reset();
			m_burstActionSecondary.Reset();

			m_configs[0].weaponMode = EWeaponMode.Primary;
			m_configs[0].weaponState = primaryWeaponState;
			m_configs[0].weaponConfig = weaponDefinition?.primaryFire;
			m_configs[0].ammoInterface?.Reset(ammoSource,
											  weaponDefinition,
											  m_configs[0].weaponMode,
											  m_configs[0].weaponConfig,
											  primaryWeaponState);

			m_configs[1].weaponMode = EWeaponMode.Secondary;
			m_configs[1].weaponState = secondaryWeaponState;
			m_configs[1].weaponConfig = null;
			if (weaponDefinition != null && weaponDefinition.secondaryFireEnabled)
			{
				m_configs[1].weaponConfig = weaponDefinition.secondaryFire;
			}
			m_configs[1].ammoInterface?.Reset(ammoSource,
											  weaponDefinition,
											  m_configs[1].weaponMode,
											  m_configs[1].weaponConfig,
											  secondaryWeaponState);
		}

		// -------------------------------------------------------------------------------------------------

		private WeaponInstanceConfig GetConfig(EWeaponMode weaponMode)
		{
			return m_configs[(int)weaponMode];
		}

		// -------------------------------------------------------------------------------------------------

		public void SetIsEquipped(bool equipped)
		{
			if (m_equipped != equipped)
			{
				m_equipped = equipped;

				if (!m_equipped)
				{
					// cancel any ongoing reloads or bursts when the weapon is unequipped
					m_burstActionPrimary.Reset();
					m_burstActionSecondary.Reset();
					m_configs[0].ammoInterface?.Cancel();
					m_configs[1].ammoInterface?.Cancel();
				}
			}
		}

		// -------------------------------------------------------------------------------------------------

		public void UpdateWeaponInstance(float time, float delta)
		{
			if (!m_equipped)
			{
				return;
			}

			m_burstActionPrimary.Update(time);
			m_burstActionSecondary.Update(time);

			// update the ammo interface for each config
			for (int i = 0; i < m_configs.Length; ++i)
			{
				m_configs[i].ammoInterface?.UpdateAmmoInterface(time, delta);
			}

			for (int i = 0; i < m_configs.Length; ++i)
			{
				// stop updating configs if a blocking reload is in progress
				if (IsBlockingReloadInProgress())
				{
					break;
				}

				// don't do anything if the weapon state is null
				WeaponInstanceConfig config = m_configs[i];
				if (config.weaponState == null)
				{
					continue;
				}

				// don't update trigger behaviour for this config if a burst fire or non-blocking
				// reload is in progress (for this config)
				bool reloadInProgress = config.ammoInterface != null ?
					config.ammoInterface.IsReloadInProgress() : false;
				bool burstInProgress = (config.weaponMode == EWeaponMode.Primary) ?
					m_burstActionPrimary.isActive : m_burstActionSecondary.isActive;

				if (config.weaponConfig != null && !burstInProgress && !reloadInProgress)
				{
					// run trigger behaviour
					if (config.weaponConfig.triggerConfig != null)
					{
						TriggerBehaviourConfig triggerConfig = config.weaponConfig.triggerConfig;
						if (triggerConfig != null && triggerConfig.behaviour != null)
						{
							triggerConfig.behaviour.UpdateTriggerBehaviour(time,
																		   delta,
																		   config.weaponMode,
																		   config.weaponConfig,
																		   config.weaponState,
																		   config.ammoInterface,
																		   triggerConfig.settings,
																		   m_cachedActivateDelegate);
						}
					}
				}
			}
		}

		// -------------------------------------------------------------------------------------------------

		public bool IsBlockingReloadInProgress()
		{
			for (int i = 0; i < m_configs.Length; ++i)
			{
				if (m_configs[0].ammoInterface != null &&
					m_configs[0].ammoInterface.IsBlockingReloadInProgress())
				{
					return true;
				}
			}

			return false;
		}

		// -------------------------------------------------------------------------------------------------

		public void TriggerPulled(EWeaponMode weaponMode)
		{
			WeaponInstanceConfig config = GetConfig(weaponMode);
			if (config.weaponConfig != null && config.weaponState != null && m_equipped)
			{
				m_weaponEvents?.TriggerTriggerChangedEvents(m_weapon, weaponMode, true);
				config.weaponState.SetTriggerIsHeld(true);
				config.weaponState.SetLastTriggerPullTime(Time.time);
			}
		}

		// -------------------------------------------------------------------------------------------------

		public void TriggerReleased(EWeaponMode weaponMode)
		{
			WeaponInstanceConfig config = GetConfig(weaponMode);
			if (config.weaponConfig != null && config.weaponState != null && m_equipped)
			{
				m_weaponEvents?.TriggerTriggerChangedEvents(m_weapon, weaponMode, false);
				config.weaponState.SetTriggerIsHeld(false);
			}
		}

		// -------------------------------------------------------------------------------------------------

		public void OnManualReload()
		{
			if (!m_equipped)
			{
				return;
			}

			// already a blocking reload in progress
			if (IsBlockingReloadInProgress())
			{
				return;
			}

			GetConfig(EWeaponMode.Primary).ammoInterface?.StartReload(EReloadType.Manual, true);
			GetConfig(EWeaponMode.Secondary).ammoInterface?.StartReload(EReloadType.Manual, true);
		}

		// -------------------------------------------------------------------------------------------------

		private void OnAutoReload(EWeaponMode weaponMode)
		{
			// already a blocking reload in progress
			if (IsBlockingReloadInProgress())
			{
				return;
			}

			WeaponInstanceConfig config = GetConfig(weaponMode);
			bool blockingReload = config.weaponConfig.ammoConfig.disableWeaponDuringAutoReload;
			config.ammoInterface?.StartReload(EReloadType.Automatic, blockingReload);
		}

		// -------------------------------------------------------------------------------------------------

		public void Activate(EWeaponMode weaponMode, bool succeeded)
		{
			if (!m_equipped)
			{
				return;
			}

			WeaponInstanceConfig config = GetConfig(weaponMode);
			WeaponConfig weaponConfig = config.weaponConfig;
			if (weaponConfig == null)
			{
				return;
			}

			// update weapon state
			if (config.weaponState != null)
			{
				config.weaponState.SetActivationCount(config.weaponState.activationCount + 1);
				config.weaponState.SetLastActivationTime(Time.time);
			}

			// return early if this was a failed activation
			if (!succeeded)
			{
				m_weaponEvents?.TriggerActivationEvents(m_weapon, weaponMode, false);
				return;
			}

			// query the ammo interface to check ammo requirements are met for this activation
			if (config.ammoInterface != null &&
				!config.ammoInterface.ConsumeAmmo(weaponConfig.ammoConfig.ammoUsedPerActivation))
			{
				// ammo check failed
				m_weaponEvents?.TriggerActivationEvents(m_weapon, weaponMode, false);

				// do we need to trigger an automatic reload?
				if (config.ammoInterface.IsReloadAllowed(EReloadType.Automatic) &&
					config.ammoInterface.NeedsReload())
				{
					OnAutoReload(weaponMode);
				}

				return;
			}

			// notify of activation
			m_weaponEvents?.TriggerActivationEvents(m_weapon, weaponMode, true);

			if (FireShot(weaponMode))
			{
				// if burst mode is enabled, schedule additional shots
				if (weaponConfig.burstConfig.isEnabled)
				{
					int burstCount = 0;
					if (weaponConfig.burstConfig.useTriggerCharges)
					{
						burstCount = config.weaponState.currentTriggerCharges;
					}
					else
					{
						burstCount = weaponConfig.burstConfig.burstShotCount;
					}

					if (burstCount > 1)
					{
						ScheduledAction<EWeaponMode> burstAction = (weaponMode == EWeaponMode.Primary) ?
							m_burstActionPrimary : m_burstActionSecondary;

						burstAction.Schedule(Time.time,
											 weaponConfig.burstConfig.burstShotInterval,
											 weaponMode,
											 burstCount - 1);
					}
				}
			}
		}

		// -------------------------------------------------------------------------------------------------

		private bool FireShot(EWeaponMode weaponMode)
		{
			if (!m_equipped)
			{
				return false;
			}

			// if no weapon config is set in the weapon definition, ignore this
			WeaponInstanceConfig config = GetConfig(weaponMode);
			WeaponConfig weaponConfig = config.weaponConfig;

			// update weapon state
			if (config.weaponState != null)
			{
				config.weaponState.SetShotCount(config.weaponState.shotCount + 1);
				config.weaponState.SetLastShotTime(Time.time);
			}

			// query the ammo interface to check ammo requirements are met for this activation
			if (config.ammoInterface != null &&
				!config.ammoInterface.ConsumeAmmo(weaponConfig.ammoConfig.ammoUsedPerShot))
			{
				// ammo check failed
				m_weaponEvents?.TriggerShotEvents(m_weapon, weaponMode, false);

				// do we need to trigger an automatic reload?
				if (config.ammoInterface.IsReloadAllowed(EReloadType.Automatic) &&
					config.ammoInterface.NeedsReload())
				{
					OnAutoReload(weaponMode);
				}

				return false;
			}

			// fire all shots
			ProjectileDefinition projectile = weaponConfig.projectile;
			if (projectile != null)
			{
				// is multi-shot enabled?  If so how many shots do we need to fire
				int shotCount = 1;
				if (weaponConfig.multiShotConfig.multiShotEnabled)
				{
					if (weaponConfig.multiShotConfig.useTriggerCharges)
					{
						shotCount = config.weaponState.currentTriggerCharges;
					}
					else
					{
						shotCount = weaponConfig.multiShotConfig.multiShotCount;
					}
				}

				// cache shot-spread config
				#if FULLY_LOADED_2D
				ShotSpreadBehaviourConfig2D shotSpreadConfig = config.weaponConfig.shotSpreadConfig2D;
				#else
				ShotSpreadBehaviourConfig shotSpreadConfig = config.weaponConfig.shotSpreadConfig;
				#endif

				// fire all shots
				for (int i = 0; i < shotCount; ++i)
				{
					// shot direction may be randomized
					Vector3 direction = m_weaponAim.GetDirection();
					if (shotSpreadConfig != null && shotSpreadConfig.behaviour != null)
					{
						#if FULLY_LOADED_2D

						// apply 2D shot spread
						direction = shotSpreadConfig.behaviour.GetRandomizedDirection(direction,
							                                                          i,
																					  shotCount,
																					  shotSpreadConfig.settings);

						#else

						// apply 3D shot spread
						Vector3 rightVector = Vector3.Cross(direction, Vector3.up).normalized;
						Vector3 upVector = Vector3.Cross(direction, rightVector).normalized;
						direction = shotSpreadConfig.behaviour.GetRandomizedDirection(direction,
							                                                          upVector,
																					  rightVector,
																					  i,
																					  shotCount,
																					  shotSpreadConfig.settings);
						#endif
					}

					// instant-hit: perform the raycast and notify any target
					if (projectile.type == EShotType.InstantHit)
					{
						Vector3 finalPos;
						bool validHit = InstantHit.ProcessInstantHit(m_weaponAim.GetSourcePosition(),
							                                         direction,
																	 projectile,
																	 m_owner,
																	 out finalPos);

						// if set up for area damage and we got a final position, deal aoe damage
						if (validHit && projectile.explodeOnImpact)
						{
							AreaEffect.DealExplosionDamage(finalPos, projectile, m_owner);

							// spawn explosion effect
							if (projectile.explosionEffect != null)
							{
								SpawnedEffect.Instantiate(projectile.explosionEffect, finalPos, Quaternion.identity);
							}
						}

						// spawn the instant-hit trail effect
						if (projectile.instantHitTrailPrefab != null)
						{
							InstantHitTrail trail = InstantHitTrail.Instantiate(projectile.instantHitTrailPrefab);
							if (trail != null)
							{
								Vector3 targetPoint = (validHit)
									? finalPos
									: m_weaponAim.GetSourcePosition() + (direction * projectile.maxDistance);

								trail.Initialize(m_weaponAim.GetSourcePosition(), targetPoint);
							}
						}
					}

					// projectile: spawn the projectile
					else if (projectile.type == EShotType.Projectile)
					{
						if (projectile.projectilePrefab != null)
						{
							GameObject obj = ObjectPoolManager.instance.SpawnObject(projectile.projectilePrefab, m_weaponAim.GetSourcePosition(), Quaternion.identity);
							if (obj != null)
							{
								BaseProjectile proj = obj.GetComponent<BaseProjectile>();
								if (proj != null)
								{
									proj.Initialize(direction, projectile, m_owner);
								}
							}
						}
					}
				}
			}

			// notify of shot success
			m_weaponEvents?.TriggerShotEvents(m_weapon, weaponMode, true);
			return true;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
