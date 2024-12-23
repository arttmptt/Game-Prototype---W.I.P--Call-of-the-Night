using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace FullyLoaded
{
    [DisallowMultipleComponent]
    public class WeaponBag : MonoBehaviour
    {
        // -------------------------------------------------------------------------------------------------

        [System.Serializable]
        public class WeaponBagConfig
        {
            [SerializeField] private WeaponDefinition m_weapon = null;
            [SerializeField] private bool m_initiallyOwned = false;

            public WeaponDefinition weapon { get { return m_weapon; } }
            public bool isInitiallyOwned { get { return m_initiallyOwned; } }
        }

        // -------------------------------------------------------------------------------------------------

        [SerializeField] private List<WeaponBagConfig> m_weaponSetup = new List<WeaponBagConfig>();

        private List<WeaponDefinition> m_ownedWeapons = new List<WeaponDefinition>();
        private List<WeaponDefinition> m_validWeapons = new List<WeaponDefinition>();
        private Dictionary<WeaponDefinition, bool> m_ownershipState = new Dictionary<WeaponDefinition, bool>();

        public ReadOnlyCollection<WeaponDefinition> validWeapons { get { return m_validWeapons.AsReadOnly(); } }
        public ReadOnlyCollection<WeaponDefinition> ownedWeapons { get { return m_ownedWeapons.AsReadOnly(); } }

        public delegate void OwnershipChangedDelegate(WeaponDefinition weapon, bool newOwnership);
        public event OwnershipChangedDelegate m_onOwnershipChanged = null;

        // -------------------------------------------------------------------------------------------------

        private void Awake()
        {
            // initialize the owned weapons dictionary and list, remembering any duplicates so that we can
            // remove them afterwards
            List<int> duplicates = new List<int>();
            for (int i = 0; i < m_weaponSetup.Count; ++i)
            {
                bool initiallyOwned = m_weaponSetup[i].isInitiallyOwned;
                WeaponDefinition weapon = m_weaponSetup[i].weapon;

                if (weapon != null)
                {
                    if (m_ownershipState.ContainsKey(weapon))
                    {
                        duplicates.Add(i);
                        Debug.LogWarning($"WeaponBag::Awake: duplicate weapon");
                    }
                    else
                    {
                        m_ownershipState[weapon] = initiallyOwned;

                        m_validWeapons.Add(weapon);
                        if (initiallyOwned)
                        {
                            m_ownedWeapons.Add(weapon);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"WeaponBag::Awake: invalid weapon");
                }
            }

            // remove any duplicate weapon entries
            for (int i = duplicates.Count - 1; i >= 0; --i)
            {
                m_weaponSetup.RemoveAt(duplicates[i]);
            }

            // remove any entries that have no weapon definition set
            m_weaponSetup.RemoveAll(x => x.weapon == null);
        }

        // -------------------------------------------------------------------------------------------------

        public bool IsValidWeapon(WeaponDefinition weapon)
        {
            return (weapon != null) ? m_ownershipState.ContainsKey(weapon) : false;
        }

        // -------------------------------------------------------------------------------------------------

        public bool IsOwned(WeaponDefinition weapon)
        {
            if (weapon != null && m_ownershipState.TryGetValue(weapon, out bool owned))
            {
                return owned;
            }

            return false;
        }

        // -------------------------------------------------------------------------------------------------

        public bool SetOwnership(WeaponDefinition weapon, bool owned)
        {
            if (weapon != null && m_ownershipState.ContainsKey(weapon))
            {
                if (m_ownershipState[weapon] != owned)
                {
                    m_ownershipState[weapon] = owned;
                    if (owned)
                    {
                        m_ownedWeapons.Add(weapon);
                    }
                    else
                    {
                        m_ownedWeapons.Remove(weapon);
                    }

                    if (m_onOwnershipChanged != null)
                    {
                        m_onOwnershipChanged(weapon, owned);
                    }

                    return true;
                }
            }

            return false;
        }

        // -------------------------------------------------------------------------------------------------

        public WeaponDefinition GetNextWeapon(WeaponDefinition current, bool onlyIncludeOwned)
        {
            int currentIndex = m_validWeapons.FindIndex(x => x == current);
            if (current == null || currentIndex == -1)
            {
                // if the current weapon passed in was invalid, start at the beginning of the list
                currentIndex = -1;
            }

            WeaponDefinition weapon = current;
            for (int i = 0; i < m_validWeapons.Count; ++i)
            {
                if (++currentIndex == m_validWeapons.Count)
                {
                    currentIndex = 0;
                }

                if (onlyIncludeOwned && !m_ownershipState[m_validWeapons[currentIndex]])
                {
                    continue;
                }

                return m_validWeapons[currentIndex];
            }

            return null;
        }

        // -------------------------------------------------------------------------------------------------

        public WeaponDefinition GetPrevWeapon(WeaponDefinition current, bool onlyIncludeOwned)
        {
            int currentIndex = m_validWeapons.FindIndex(x => x == current);
            if (current == null || currentIndex == -1)
            {
                // if the current weapon passed in was invalid, start at the beginning of the list
                currentIndex = 1;
            }

            WeaponDefinition weapon = current;
            for (int i = 0; i < m_validWeapons.Count; ++i)
            {
                if (--currentIndex == -1)
                {
                    currentIndex = m_validWeapons.Count - 1;
                }

                if (onlyIncludeOwned && !m_ownershipState[m_validWeapons[currentIndex]])
                {
                    continue;
                }

                return m_validWeapons[currentIndex];
            }

            return null;
        }

        // -------------------------------------------------------------------------------------------------
    }
}
