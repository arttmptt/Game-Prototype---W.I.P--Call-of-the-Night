using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(MultiShotConfig))]
	public class MultiShotConfigPropertyDrawer : PropertyDrawer
	{
		// -------------------------------------------------------------------------------------------------

		private VisualElement m_multiShotSettings = null;

		// -------------------------------------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();
			root.AddToClassList("MultiShotConfig");

			// this property determines if the group below is enabled/disabled for editing
			PropertyField isEnabled = new PropertyField(property.FindPropertyRelative("m_multiShotEnabled"));
			root.Add(isEnabled);

			PropertyField multiShotCount = new PropertyField(property.FindPropertyRelative("m_multiShotCount"));
			PropertyField useTriggerCharges = new PropertyField(property.FindPropertyRelative("m_useTriggerCharges"));

			// add the following properties to a container element so they can be enabled/disabled as one
			m_multiShotSettings = new VisualElement();
			root.Add(m_multiShotSettings);
			m_multiShotSettings.Add(useTriggerCharges);
			m_multiShotSettings.Add(multiShotCount);

			// register for change callbacks

			// when m_multiShotEnabled changes, update the enabled/disabled state of the multi-shot settings
			isEnabled.RegisterValueChangeCallback(evt =>
			{
				m_multiShotSettings.SetEnabled(evt.changedProperty.boolValue);
			});

			// when m_useTriggerCharges changes, update the enabled/disabled state of m_multiShotCount
			useTriggerCharges.RegisterValueChangeCallback(evt =>
			{
				multiShotCount.SetEnabled(!evt.changedProperty.boolValue);
			});

			// set initial enabled state of the multi-shot settings
			SerializedProperty isEnabledProp = property.FindPropertyRelative("m_multiShotEnabled");
			if (isEnabledProp != null)
			{
				m_multiShotSettings.SetEnabled(isEnabledProp.boolValue);
			}

			// set initial enabled state of m_multiShotCount
			SerializedProperty useTriggerChargesProp = property.FindPropertyRelative("m_useTriggerCharges");
			if (useTriggerChargesProp != null)
			{
				multiShotCount.SetEnabled(!useTriggerChargesProp.boolValue);
			}

			return root;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
