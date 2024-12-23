using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[DisallowMultipleComponent]
	public class AmmoBelt : MonoBehaviour, IAmmoSource
	{
		// -------------------------------------------------------------------------------------------------

		[System.Serializable]
		public struct AmmoBeltConfig
		{
			[SerializeField] private AmmoType m_type;
			[SerializeField] private int m_initialAmount;
			[SerializeField] private int m_maxAmount;
			[SerializeField] private bool m_isInfinite;

			public AmmoType type { get { return m_type; } }
			public int initialAmount { get { return m_initialAmount; } }
			public int maxAmount { get { return m_maxAmount; } }
			public bool isInfinite { get { return m_isInfinite; } }
		}

		[SerializeField] private List<AmmoBeltConfig> m_ammoConfig = new List<AmmoBeltConfig>();

		private Dictionary<AmmoType, AmmoBeltConfig> m_configLookup = new Dictionary<AmmoType, AmmoBeltConfig>();
		private Dictionary<AmmoType, int> m_ammoCountLookup = new Dictionary<AmmoType, int>();
		private Dictionary<AmmoType, bool> m_isInfiniteLookup = new Dictionary<AmmoType, bool>();

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			// build the lookup dictionaries
			for (int i = 0; i < m_ammoConfig.Count; ++i)
			{
				if (m_ammoConfig[i].type == null)
				{
					Debug.LogWarning($"AmmoBelt: null AmmoType in config");
				}
				else if (m_configLookup.ContainsKey(m_ammoConfig[i].type))
				{
					Debug.LogWarning($"AmmoBelt: duplicate AmmoType in config");
				}
				else
				{
					m_configLookup.Add(m_ammoConfig[i].type, m_ammoConfig[i]);
					m_ammoCountLookup.Add(m_ammoConfig[i].type, m_ammoConfig[i].initialAmount);
					m_isInfiniteLookup.Add(m_ammoConfig[i].type, m_ammoConfig[i].isInfinite);
				}
			}
		}

		// -------------------------------------------------------------------------------------------------

		public int GetAmmoCount(AmmoType type)
		{
			if (type == null || !m_ammoCountLookup.ContainsKey(type))
			{
				return 0;
			}

			return m_ammoCountLookup[type];
		}

		// -------------------------------------------------------------------------------------------------

		public int GetMaxAmmoCount(AmmoType type)
		{
			if (type == null || !m_configLookup.ContainsKey(type))
			{
				return 0;
			}

			return m_configLookup[type].maxAmount;
		}

		// -------------------------------------------------------------------------------------------------

		public bool HasInfiniteAmmo(AmmoType type)
		{
			if (type == null || !m_isInfiniteLookup.ContainsKey(type))
			{
				return false;
			}

			return m_isInfiniteLookup[type];
		}

		// -------------------------------------------------------------------------------------------------

		public bool SetInfiniteAmmo(AmmoType type, bool isInfinite)
		{
			if (type == null || !m_isInfiniteLookup.ContainsKey(type))
			{
				return false;
			}

			m_isInfiniteLookup[type] = isInfinite;
			return true;
		}

		// -------------------------------------------------------------------------------------------------

		public bool SetAmmoCount(AmmoType type, int ammoCount)
		{
			if (type == null || !m_ammoCountLookup.ContainsKey(type) || !m_configLookup.ContainsKey(type))
			{
				return false;
			}

			m_ammoCountLookup[type] = Mathf.Min(ammoCount, m_configLookup[type].maxAmount);
			return true;
		}

		// -------------------------------------------------------------------------------------------------

		public int AddAmmo(AmmoType type, int amount)
		{
			if (type == null || !m_ammoCountLookup.ContainsKey(type) || !m_configLookup.ContainsKey(type))
			{
				return 0;
			}

			if (amount > 0)
			{
				int prevAmount = m_ammoCountLookup[type];
				m_ammoCountLookup[type] = Mathf.Min(m_ammoCountLookup[type] + amount, m_configLookup[type].maxAmount);
				return Mathf.Max(0, m_ammoCountLookup[type] - prevAmount);
			}

			return 0;
		}

		// -------------------------------------------------------------------------------------------------

		public int RemoveAmmo(AmmoType type, int amount)
		{
			if (type == null || !m_ammoCountLookup.ContainsKey(type))
			{
				return 0;
			}

			if (amount > 0)
			{
				int prevAmount = m_ammoCountLookup[type];
				m_ammoCountLookup[type] = Mathf.Max(m_ammoCountLookup[type] - amount, 0);
				return Mathf.Max(0, prevAmount - m_ammoCountLookup[type]);
			}

			return 0;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
