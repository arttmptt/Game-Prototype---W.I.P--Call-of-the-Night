using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(TriggerBehaviourSettings_HoldToCharge))]
	public class HoldToChargePropertyDrawer : PropertyDrawer
	{
		// -------------------------------------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();
			root.AddToClassList("HoldToChargeSettings");

			// settings properties
			root.Add(new PropertyField(property.FindPropertyRelative("m_minTriggerCharges")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_maxTriggerCharges")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_timeToMinTriggerCharges")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_timeToMaxTriggerCharges")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_ammoUsedPerCharge")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_autoFireAtMaxCharges")));

			return root;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
