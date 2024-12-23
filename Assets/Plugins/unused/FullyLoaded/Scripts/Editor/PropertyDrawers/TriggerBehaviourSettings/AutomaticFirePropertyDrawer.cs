using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(TriggerBehaviourSettings_AutomaticFire))]
	public class AutomaticFirePropertyDrawer : PropertyDrawer
	{
		// -------------------------------------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();
			root.AddToClassList("AutomaticFireSettings");

			// settings properties
			root.Add(new PropertyField(property.FindPropertyRelative("m_spinUpTime")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_rateOfFire")));

			return root;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
