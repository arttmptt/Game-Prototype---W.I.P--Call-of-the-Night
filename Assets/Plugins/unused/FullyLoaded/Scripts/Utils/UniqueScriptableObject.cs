using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public abstract class UniqueScriptableObject : ScriptableObject
    {
        // -------------------------------------------------------------------------------------------------

        [System.Serializable]
        public class UniqueId
        {
            [SerializeField] private string m_uniqueId = "";
            public string uniqueId { get { return m_uniqueId; } }
        }

        // -------------------------------------------------------------------------------------------------

        // internal asset version number
        [SerializeField, HideInInspector] private int m_assetVersion = AssetVersion.versionNumber;
        protected int assetVersion { get { return m_assetVersion; } }

        // unique asset ID
        [SerializeField] private UniqueId m_id = new UniqueId();
        public string uniqueId { get { return m_id.uniqueId; } }

        // -------------------------------------------------------------------------------------------------

        protected virtual bool UpgradeVersion(int versionOnDisk, int newVersion)
        {
            return true;
        }

        // -------------------------------------------------------------------------------------------------

        private void OnValidate()
        {
            #if UNITY_EDITOR
            if (assetVersion < AssetVersion.versionNumber)
            {
                if (UpgradeVersion(assetVersion, AssetVersion.versionNumber))
                {
                    m_assetVersion = AssetVersion.versionNumber;
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.AssetDatabase.SaveAssets();
                }
            }
            #endif
        }

        // -------------------------------------------------------------------------------------------------
    }
}
