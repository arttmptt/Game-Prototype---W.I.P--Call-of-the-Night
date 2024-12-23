using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(ShotSpreadBehaviourSettings_Cone2D))]
	public class Cone2DSpreadPropertyDrawer : PropertyDrawer
	{
		// -------------------------------------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();
			root.AddToClassList("Cone2DSpreadSettings");

			// settings properties
			root.Add(new PropertyField(property.FindPropertyRelative("m_maxAngle")));
			root.Add(new PropertyField(property.FindPropertyRelative("m_angleType")));

			return root;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
