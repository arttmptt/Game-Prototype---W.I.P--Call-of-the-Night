using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    [System.Serializable]
    public abstract class CustomBehaviourConfig<BehaviourAssetType, SettingsType>
    where BehaviourAssetType : CustomBehaviourAsset, ICustomBehaviourSettingsSpawner<SettingsType>
    where SettingsType : CustomBehaviourSettings
    {
        // -------------------------------------------------------------------------------------------------

        [SerializeReference] private SettingsType m_settings = null;

        public SettingsType settings { get { return m_settings; } }
        public abstract BehaviourAssetType behaviour { get; }

        // -------------------------------------------------------------------------------------------------

        public abstract string GetDisplayName();

        // -------------------------------------------------------------------------------------------------
    }
}
