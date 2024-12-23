using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(ShotSpreadBehaviourSettings_Cone))]
	public class ConeSpreadPropertyDrawer : PropertyDrawer
	{
		// -------------------------------------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();
			root.AddToClassList("ConeSpreadSettings");

			// settings properties
			root.Add(new PropertyField(property.FindPropertyRelative("m_minAngle")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_maxAngle")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_circumferentialAngleType")));

			return root;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
