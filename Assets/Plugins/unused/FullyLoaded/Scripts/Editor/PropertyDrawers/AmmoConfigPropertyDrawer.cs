using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(AmmoConfig))]
	public class AmmoConfigPropertyDrawer : PropertyDrawer
	{
		// -------------------------------------------------------------------------------------------------

		private VisualElement m_clipSettings = null;

		// -------------------------------------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();
			root.AddToClassList("AmmoConfig");

			root.Add(new PropertyField(property.FindPropertyRelative("m_ammoUsedPerActivation")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_ammoUsedPerShot")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_ammoType")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_isInfinite")));

			// this property determines if the group below is enabled/disabled for editing
			PropertyField hasClipField = new PropertyField(property.FindPropertyRelative("m_hasReloadableClip"));
			root.Add(hasClipField);

			// add the following properties to a container element so they can be enabled/disabled as one
			m_clipSettings = new VisualElement();
			root.Add(m_clipSettings);
			m_clipSettings.Add(new PropertyField(property.FindPropertyRelative("m_clipSize")));
			m_clipSettings.Add(new PropertyField(property.FindPropertyRelative("m_reloadTime")));
			m_clipSettings.Add(new PropertyField(property.FindPropertyRelative("m_allowManualReload")));
			m_clipSettings.Add(new PropertyField(property.FindPropertyRelative("m_allowAutoReload")));
			m_clipSettings.Add(new PropertyField(property.FindPropertyRelative("m_disableWeaponDuringAutoReload")));

			// register for change callbacks
			// when m_hasReloadableClip changes, update the enabled/disabled state of the clip/reload settings
			hasClipField.RegisterValueChangeCallback(evt =>
			{
				SetClipSettingsEnabled(evt.changedProperty.boolValue);
			});

			// set initial enabled state of the clip/reload settings
			SerializedProperty hasReloadableClipProp = property.FindPropertyRelative("m_hasReloadableClip");
			if (hasReloadableClipProp != null)
			{
				SetClipSettingsEnabled(hasReloadableClipProp.boolValue);
			}

			return root;
		}

		// -------------------------------------------------------------------------------------------------

		private void SetClipSettingsEnabled(bool enabled)
		{
			m_clipSettings.SetEnabled(enabled);
		}

		// -------------------------------------------------------------------------------------------------
	}
}
