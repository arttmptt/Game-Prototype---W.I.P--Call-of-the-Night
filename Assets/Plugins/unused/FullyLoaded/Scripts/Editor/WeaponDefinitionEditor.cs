using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
    [CustomEditor(typeof(WeaponDefinition))]
    public class WeaponDefinitionEditor : Editor
    {
        // -------------------------------------------------------------------------------------------------

        [SerializeField] private VisualTreeAsset m_visualTreeAsset = null;
        [SerializeField] private StyleSheet m_styleSheetAsset = null;

        private VisualElement m_primaryFireElement = null;
        private VisualElement m_secondaryFireElement = null;

        // -------------------------------------------------------------------------------------------------

        public override VisualElement CreateInspectorGUI()
        {
            WeaponDefinition weaponDefinition = target as WeaponDefinition;

            CustomVisualElement root = new CustomVisualElement();
            root.SetId(weaponDefinition.GetInstanceID().ToString());

            // import uxml and stylesheet
            m_visualTreeAsset?.CloneTree(root);
            if (m_styleSheetAsset != null)
            {
                root.styleSheets.Add(m_styleSheetAsset);
            }

            // register the callback to update the secondary fire config visibility when the bool changes
            Toggle secondaryFireEnabled = root.Query<Toggle>("secondaryFireEnabled");
            if (secondaryFireEnabled != null)
            {
                secondaryFireEnabled.RegisterValueChangedCallback(ctx =>
                {
                    UpdateSecondaryFireVisibility(ctx.newValue);
                });
            }

            // add custom WeaponConfigElement for primary fire
            m_primaryFireElement = root.Query<VisualElement>("primaryFire");
            if (m_primaryFireElement != null)
            {
                CustomVisualElement customElement = new CustomVisualElement();
                customElement.SetId("primary_fire");

                m_primaryFireElement.Add(customElement);
                customElement.Add(new WeaponConfigElement(EWeaponMode.Primary, weaponDefinition.primaryFire, serializedObject.FindProperty("m_primaryFireConfig")));
            }

            // add custom WeaponConfigElement for secondary fire
            m_secondaryFireElement = root.Query<VisualElement>("secondaryFire");
            if (m_secondaryFireElement != null)
            {
                CustomVisualElement customElement = new CustomVisualElement();
                customElement.SetId("secondary_fire");

                m_secondaryFireElement.Add(customElement);
                customElement.Add(new WeaponConfigElement(EWeaponMode.Secondary, weaponDefinition.secondaryFire, serializedObject.FindProperty("m_secondaryFireConfig")));
            }

            UpdateSecondaryFireVisibility(weaponDefinition.secondaryFireEnabled);
            return root;
        }

        // -------------------------------------------------------------------------------------------------

        private void UpdateSecondaryFireVisibility(bool visible)
        {
            m_secondaryFireElement.style.display = (visible) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        // -------------------------------------------------------------------------------------------------
    }
}
