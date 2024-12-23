using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(BurstConfig))]
	public class BurstConfigPropertyDrawer : PropertyDrawer
	{
		// -------------------------------------------------------------------------------------------------

		private VisualElement m_burstSettings = null;

		// -------------------------------------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();
			root.AddToClassList("BurstConfig");

			// this property determines if the group below is enabled/disabled for editing
			PropertyField isEnabled = new PropertyField(property.FindPropertyRelative("m_isEnabled"));
			root.Add(isEnabled);

			// useTriggerCharges determines if the m_burstShotCount is enabled/disabled for editing
			PropertyField burstShotCount = new PropertyField(property.FindPropertyRelative("m_burstShotCount"));
			PropertyField useTriggerCharges = new PropertyField(property.FindPropertyRelative("m_useTriggerCharges"));

			// add the following properties to a container element so they can be enabled/disabled as one
			m_burstSettings = new VisualElement();
			root.Add(m_burstSettings);
			m_burstSettings.Add(useTriggerCharges);
			m_burstSettings.Add(burstShotCount);
			m_burstSettings.Add(new PropertyField(property.FindPropertyRelative("m_burstShotInterval")));
			m_burstSettings.Add(new PropertyField(property.FindPropertyRelative("m_minTimeBetweenBursts")));

			// register for change callbacks
			// when m_isEnabled changes, update the enabled/disabled state of the burst settings
			isEnabled.RegisterValueChangeCallback(evt =>
			{
				m_burstSettings.SetEnabled(evt.changedProperty.boolValue);
			});

			// when m_useTriggerCharges changes, update the enabled/disabled state of the m_burstShotCount
			useTriggerCharges.RegisterValueChangeCallback(evt =>
			{
				burstShotCount.SetEnabled(!evt.changedProperty.boolValue);
			});

			// set initial enabled state of the burst settings
			SerializedProperty isEnabledProp = property.FindPropertyRelative("m_isEnabled");
			if (isEnabledProp != null)
			{
				m_burstSettings.SetEnabled(isEnabledProp.boolValue);
			}

			// set initial enabled state of the shot count
			SerializedProperty useTriggerChargesProp = property.FindPropertyRelative("m_useTriggerCharges");
			if (useTriggerChargesProp != null)
			{
				burstShotCount.SetEnabled(!useTriggerChargesProp.boolValue);
			}

			return root;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
