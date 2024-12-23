using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	public class CustomBehaviourConfigElement<BehaviourAssetType, SettingsType> : BindableElement
	where BehaviourAssetType : CustomBehaviourAsset, ICustomBehaviourSettingsSpawner<SettingsType>
	where SettingsType : CustomBehaviourSettings
	{
		// -------------------------------------------------------------------------------------------------

		private SerializedProperty m_settingsProp = null;
		private PropertyField m_settingsElement = null;

		// -------------------------------------------------------------------------------------------------

		public CustomBehaviourConfigElement(CustomBehaviourConfig<BehaviourAssetType, SettingsType> target, SerializedProperty property)
		{
			bindingPath = property.propertyPath;

			// import uxml
			VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/FullyLoaded/Scripts/Editor/CustomBehaviourConfigElement.uxml");
			uxml.CloneTree(this);

			// cache the config property/element
			m_settingsElement = this.Query<PropertyField>("Settings");
			m_settingsProp = property.FindPropertyRelative(m_settingsElement.bindingPath);

			// register a callback for when the behaviour asset changes
			PropertyField behaviour = this.Query<PropertyField>("Behaviour");
			if (behaviour != null)
			{
				behaviour.RegisterValueChangeCallback(ctx =>
				{
					UpdateSettings(target, ctx.changedProperty.serializedObject, true);
				});
				behaviour.RegisterCallback<SerializedPropertyChangeEvent>(evt =>
				{
					UpdateSettings(target, evt.changedProperty.serializedObject, true);
				});
			}

			UpdateSettings(target, property.serializedObject, false);
		}

		// -------------------------------------------------------------------------------------------------

		private void UpdateSettings(CustomBehaviourConfig<BehaviourAssetType, SettingsType> config, SerializedObject serializedObj, bool force)
		{
			if (m_settingsElement == null || m_settingsProp == null)
			{
				return;
			}

			if (((config.behaviour == null) != (config.settings == null)) || force)
			{
				// recreate the settings instance
				serializedObj.Update();
				m_settingsProp.managedReferenceValue = (config.behaviour != null) ? config.behaviour.CreateBehaviourSettingsInstance() : null;
				serializedObj.ApplyModifiedProperties();

				// rebind the property field
				m_settingsElement.BindProperty(m_settingsProp);
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
