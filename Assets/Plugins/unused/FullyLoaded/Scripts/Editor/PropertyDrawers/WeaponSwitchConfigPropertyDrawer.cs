using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(WeaponSwitchConfig))]
	public class WeaponSwitchConfigPropertyDrawer : PropertyDrawer
	{
		// -------------------------------------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();
			root.AddToClassList("WeaponSwitchConfig");

			root.Add(new PropertyField(property.FindPropertyRelative("m_switchInTime")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_switchOutTime")));

			return root;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
