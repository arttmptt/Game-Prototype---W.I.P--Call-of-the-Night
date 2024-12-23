using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(TriggerBehaviourSettings_SemiAutoFire))]
	public class SemiAutoFirePropertyDrawer : PropertyDrawer
	{
		// -------------------------------------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();
			root.AddToClassList("SemiAutoFireSettings");

			// settings properties
			root.Add(new PropertyField(property.FindPropertyRelative("m_maxRateOfFire")));

			return root;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
