using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(ShotSpreadBehaviourSettings_Box))]
	public class BoxSpreadPropertyDrawer : PropertyDrawer
	{
		// -------------------------------------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();
			root.AddToClassList("BoxSpreadSettings");

			// settings properties
			root.Add(new PropertyField(property.FindPropertyRelative("m_verticalAngleType")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_minVerticalAngle")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_maxVerticalAngle")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_horizontalAngleType")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_minHorizontalAngle")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_maxHorizontalAngle")));

			return root;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
