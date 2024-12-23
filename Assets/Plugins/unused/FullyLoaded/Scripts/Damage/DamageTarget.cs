using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FullyLoaded
{
	[DisallowMultipleComponent]
	public class DamageTarget : MonoBehaviour
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private bool m_allowDamage = true;
		[SerializeField] private UnityEvent<Vector3, float, DamageType, BaseWeaponUser> m_OnHit = null;

		public delegate void OnHitDelegate(Vector3 impactPoint, float damage, DamageType damageType, BaseWeaponUser source);
		public event OnHitDelegate OnHit = null;

		public bool allowDamage { get { return m_allowDamage; } }

		// -------------------------------------------------------------------------------------------------

		public void SetAllowDamage(bool damageAllowed)
		{
			m_allowDamage = damageAllowed;
		}

		// -------------------------------------------------------------------------------------------------

		public void RegisterDamage(Vector3 impactPoint, float damage, DamageType damageType, BaseWeaponUser source)
		{
			if (allowDamage)
			{
				if (OnHit != null)
				{
					OnHit(impactPoint, damage, damageType, source);
				}

				m_OnHit?.Invoke(impactPoint, damage, damageType, source);
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
