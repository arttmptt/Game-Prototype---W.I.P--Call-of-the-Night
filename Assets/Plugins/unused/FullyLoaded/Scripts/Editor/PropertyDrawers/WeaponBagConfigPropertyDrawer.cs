using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(WeaponBag.WeaponBagConfig))]
	public class WeaponBagConfigPropertyDrawer : PropertyDrawer
	{
		// -------------------------------------------------------------------------------------------------

		private readonly int kFieldCount = 2;
		private readonly float kSpacing = 1.0f;

		// -------------------------------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			float fieldHeight = (position.height / (kFieldCount + 1));

			Rect rect = new Rect(position);
			rect.height = fieldHeight;

			EditorGUI.PropertyField(rect, property.FindPropertyRelative("m_weapon"));
			rect.y += fieldHeight + kSpacing;
			EditorGUI.PropertyField(rect, property.FindPropertyRelative("m_initiallyOwned"));

			EditorGUI.EndProperty();
		}

		// -------------------------------------------------------------------------------------------------

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label) * (kFieldCount + 1);
		}

		// -------------------------------------------------------------------------------------------------
	}
}
