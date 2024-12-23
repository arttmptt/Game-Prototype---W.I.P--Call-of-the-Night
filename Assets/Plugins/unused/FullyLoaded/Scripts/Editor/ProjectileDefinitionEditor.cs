using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
    [CustomEditor(typeof(ProjectileDefinition))]
    public class ProjectileDefinitionEditor : Editor
    {
        // -------------------------------------------------------------------------------------------------

        [SerializeField] private VisualTreeAsset m_visualTreeAsset = null;
        [SerializeField] private StyleSheet m_styleSheetAsset = null;

        private ProjectileDefinition m_projectileDefinition = null;
        private List<VisualElement> m_projectileGroups = new List<VisualElement>();
        private List<VisualElement> m_instantHitGroups = new List<VisualElement>();

        VisualElement m_physicsProjectile = null;
        VisualElement m_piercingProjectile = null;
        Label m_prefabWarningLabel = null;

        // -------------------------------------------------------------------------------------------------

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            m_projectileDefinition = target as ProjectileDefinition;

            // import uxml and stylesheet
            m_visualTreeAsset?.CloneTree(root);
            if (m_styleSheetAsset != null)
            {
                root.styleSheets.Add(m_styleSheetAsset);
            }

            // within the projectile settings there is a choice between showing piercing or physics settings
            // register a callback for when the prefab changes so we can check which it is
            VisualElement projectileGroup = root.Query<VisualElement>("projectile");
            if (projectileGroup != null)
            {
                m_prefabWarningLabel = projectileGroup.Query<Label>("PrefabWarningLabel");

                m_physicsProjectile = root.Query("physics");
                m_piercingProjectile = root.Query("piercing");
                PropertyField projectilePrefab = projectileGroup.Query<PropertyField>("prefab");
                projectilePrefab.RegisterValueChangeCallback(ctx =>
                {
                    UpdateProjectileType();
                });
            }

            // find all settings that are linked to either projectile or instant-hit type,
            // so we can enable/disable them depending upon the type chosen
            VisualElement explosionGroup = root.Query("explosion");
            VisualElement effectsGroup = root.Query("effects");

            m_projectileGroups.Add(root.Query("projectile"));
            m_projectileGroups.Add(explosionGroup.Query("projectile"));
            m_projectileGroups.Add(effectsGroup.Query("projectile"));
            m_instantHitGroups.Add(root.Query("instant-hit"));
            m_instantHitGroups.Add(explosionGroup.Query("instant-hit"));
            m_instantHitGroups.Add(effectsGroup.Query("instant-hit"));

            VisualElement physics = root.Query("projectile.physics");
            VisualElement piercing = root.Query("projectile.piercing");

            // register a callback for when the type changes
            PropertyField typeField = root.Query<PropertyField>("type");
            if (typeField != null)
            {
                typeField.RegisterValueChangeCallback(ctx =>
                {
                    UpdateGroupVisibility();
                });
            }

            UpdateProjectileType();
            UpdateGroupVisibility();
            return root;
        }

        // -------------------------------------------------------------------------------------------------

        private void UpdateProjectileType()
        {
            GameObject prefab = m_projectileDefinition.projectilePrefab;

            m_physicsProjectile.style.display = (prefab != null && prefab.GetComponent<PhysicsProjectile>()) ?
                DisplayStyle.Flex : DisplayStyle.None;

            m_piercingProjectile.style.display = (prefab != null && prefab.GetComponent<PiercingProjectile>()) ?
                DisplayStyle.Flex : DisplayStyle.None;

            // update the prefab warning label
            if (m_prefabWarningLabel != null)
            {
                if (prefab == null)
                {
                    m_prefabWarningLabel.text = "Warning: no projectile prefab set";
                    m_prefabWarningLabel.style.display = DisplayStyle.Flex;
                }
                else if (prefab.GetComponent<BaseProjectile>() == null)
                {
                    m_prefabWarningLabel.text = "Warning: prefab must have a Projectile component";
                    m_prefabWarningLabel.style.display = DisplayStyle.Flex;
                }
                else
                {
                    m_prefabWarningLabel.text = "";
                    m_prefabWarningLabel.style.display = DisplayStyle.None;
                }
            }
        }

        // -------------------------------------------------------------------------------------------------

        private void UpdateGroupVisibility()
        {
            List<VisualElement> listToActivate = m_instantHitGroups;
            List<VisualElement> listToDeactivate = m_projectileGroups;
            if (m_projectileDefinition.type == EShotType.Projectile)
            {
                listToActivate = m_projectileGroups;
                listToDeactivate = m_instantHitGroups;
            }

            // update element visibility
            for (int i = 0; i < listToActivate.Count; ++i)
            {
                listToActivate[i].style.display = DisplayStyle.Flex;
            }
            for (int i = 0; i < listToDeactivate.Count; ++i)
            {
                listToDeactivate[i].style.display = DisplayStyle.None;
            }
        }

        // -------------------------------------------------------------------------------------------------
    }
}
