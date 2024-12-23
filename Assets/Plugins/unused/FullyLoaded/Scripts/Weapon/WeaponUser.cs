using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace FullyLoaded
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(WeaponBag))]
	public class WeaponUser : BaseWeaponUser
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private WeaponEvents m_weaponEvents = new WeaponEvents();

		private WeaponBag m_weaponBag = null;
		private IAmmoSource m_ammoSource = null;
		private WeaponSwitcher m_weaponSwitcher = null;
		private PersistentWeaponState m_weaponState = new PersistentWeaponState();

		private bool[] m_triggerHeld = new bool[] { false, false };

		public override WeaponDefinition currentWeapon { get { return m_weaponSwitcher?.currentWeapon; } }
		public WeaponDefinition pendingWeapon { get { return m_weaponSwitcher?.pendingWeapon; } }
		public bool weaponSwitchInProgress { get { return (m_weaponSwitcher != null) ? m_weaponSwitcher.isBusy : false; } }
		public bool reloadInProgress { get { return (m_weaponInstance != null) ? m_weaponInstance.IsBlockingReloadInProgress() : false; } }
		public WeaponEvents weaponEvents { get { return m_weaponEvents; } }
		public IAmmoInfo ammoInfo { get { return m_ammoSource; } }

		public GameObject rootObject { get { return gameObject; } }

		// -------------------------------------------------------------------------------------------------

		protected override void Awake()
		{
			base.Awake();

			m_weaponBag = GetComponent<WeaponBag>();
			m_ammoSource = GetComponent<IAmmoSource>();

			// if no ammo source component is found, create an infinite ammo source
			if (m_ammoSource == null)
			{
				m_ammoSource = new InfiniteAmmoSource();
			}

			// create the weapon instance
			m_weaponInstance = new WeaponInstance(this, weaponAim, m_ammoSource, m_weaponEvents);
		}

		// -------------------------------------------------------------------------------------------------

		private void Start()
		{
			// initialize weapon switcher and register callbacks
			m_weaponSwitcher = new WeaponSwitcher(m_weaponEvents, m_weaponBag);
			m_weaponSwitcher.m_onCurrentWeaponChanged += OnCurrentWeaponChanged;
			m_weaponSwitcher.m_onWeaponSwitchStarted += OnWeaponSwitchStarted;
			m_weaponSwitcher.m_onWeaponSwitchFinished += OnWeaponSwitchFinished;

			// initialize weapon state for initially owned weapons
			if (m_weaponBag != null)
			{
				// register a callback with WeaponBag so we know when a weapon is acquired/dropped
				m_weaponBag.m_onOwnershipChanged += OnWeaponOwnershipChanged;

				// create and initialize weapon state for all initially-owned weapons
				ReadOnlyCollection<WeaponDefinition> ownedWeapons = m_weaponBag.ownedWeapons;
				for (int i = 0; i < ownedWeapons.Count; ++i)
				{
					OnWeaponOwnershipChanged(ownedWeapons[i], true);
				}
			}

			// switch to the initial weapon if possible
			if (m_initialWeapon != null && !SwitchToWeapon(m_initialWeapon))
			{
				SwitchToNextWeapon();
			}
		}

		// -------------------------------------------------------------------------------------------------

		protected override void Update()
		{
			m_weaponSwitcher?.UpdateWeaponSwitcher(Time.time, Time.deltaTime);

			base.Update();
		}

		// -------------------------------------------------------------------------------------------------

		public void PullTrigger(EWeaponMode weaponMode)
		{
			m_triggerHeld[(int)weaponMode] = true;

			m_weaponEvents.TriggerTriggerChangedEvents(currentWeapon, weaponMode, true);
			m_weaponInstance?.TriggerPulled(weaponMode);
		}

		// -------------------------------------------------------------------------------------------------

		public void ReleaseTrigger(EWeaponMode weaponMode)
		{
			m_triggerHeld[(int)weaponMode] = false;

			m_weaponEvents.TriggerTriggerChangedEvents(currentWeapon, weaponMode, false);
			m_weaponInstance?.TriggerReleased(weaponMode);
		}

		// -------------------------------------------------------------------------------------------------

		public void Reload()
		{
			m_weaponInstance?.OnManualReload();
		}

		// -------------------------------------------------------------------------------------------------

		public override bool SwitchToWeapon(WeaponDefinition newWeapon)
		{
			return m_weaponSwitcher.SwitchToWeapon(newWeapon);
		}

		// -------------------------------------------------------------------------------------------------

		public bool SwitchToNextWeapon()
		{
			return m_weaponSwitcher.SwitchToNextWeapon();
		}

		// -------------------------------------------------------------------------------------------------

		public bool SwitchToPrevWeapon()
		{
			return m_weaponSwitcher.SwitchToPrevWeapon();
		}

		// -------------------------------------------------------------------------------------------------

		private void OnCurrentWeaponChanged(WeaponDefinition previous, WeaponDefinition current)
		{
			PersistentWeaponState.WeaponStatePair state = m_weaponState.GetState(current);
			m_weaponInstance.Reset(weaponAim, m_ammoSource, current, state.primary, state.secondary);
		}

		// -------------------------------------------------------------------------------------------------

		private void OnWeaponSwitchStarted(WeaponDefinition weapon)
		{
			if (weapon != null)
			{
				m_weaponInstance.SetIsEquipped(false);

				PersistentWeaponState.WeaponStatePair state = m_weaponState.GetState(weapon);
				state.primary?.OnUnequipped();
				state.secondary?.OnUnequipped();
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void OnWeaponSwitchFinished(WeaponDefinition weapon)
		{
			if (weapon != null)
			{
				m_weaponInstance.SetIsEquipped(true);

				PersistentWeaponState.WeaponStatePair state = m_weaponState.GetState(weapon);
				state.primary?.OnEquipped(Time.time, m_triggerHeld[(int)EWeaponMode.Primary]);
				state.secondary?.OnEquipped(Time.time, m_triggerHeld[(int)EWeaponMode.Secondary]);
			}
		}

		// -------------------------------------------------------------------------------------------------

		public WeaponState GetWeaponState(EWeaponMode weaponMode)
		{
			PersistentWeaponState.WeaponStatePair pair;
			if (currentWeapon != null)
			{
				pair = m_weaponState.GetState(currentWeapon);
				return (weaponMode == EWeaponMode.Primary) ? pair.primary : pair.secondary;
			}

			return null;
		}

		// -------------------------------------------------------------------------------------------------

		public void OnWeaponOwnershipChanged(WeaponDefinition weaponDef, bool isOwned)
		{
			if (isOwned)
			{
				// create and initialize weapon state when a weapon is acquired
				m_weaponState.AddWeapon(weaponDef);
			}
			else
			{
				// reset weapon state when a weapon is dropped
				m_weaponState.ResetWeaponState(weaponDef);
			}

			// fire events
			m_weaponEvents.TriggerOwnershipChangedEvents(weaponDef, isOwned);

			if (isOwned)
			{
				// if we have just acquired a weapon yet we have no current weapon, switch to it
				if (currentWeapon == null && weaponDef != null)
				{
					SwitchToWeapon(weaponDef);
				}
			}
			else
			{
				// if we have just dropped the current weapon, switch to another
				if (weaponDef != null && currentWeapon == weaponDef)
				{
					SwitchToNextWeapon();
				}
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
