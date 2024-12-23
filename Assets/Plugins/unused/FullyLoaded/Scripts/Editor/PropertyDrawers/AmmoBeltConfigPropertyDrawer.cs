using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(AmmoBelt.AmmoBeltConfig))]
	public class AmmoBeltConfigPropertyDrawer : PropertyDrawer
	{
		// -------------------------------------------------------------------------------------------------

		private readonly int kFieldCount = 4;
		private readonly float kSpacing = 1.0f;

		// -------------------------------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			float fieldHeight = (position.height / (kFieldCount + 1));

			Rect rect = new Rect(position);
			rect.height = fieldHeight;

			EditorGUI.PropertyField(rect, property.FindPropertyRelative("m_type"));
			rect.y += fieldHeight + kSpacing;
			EditorGUI.PropertyField(rect, property.FindPropertyRelative("m_initialAmount"));
			rect.y += fieldHeight + kSpacing;
			EditorGUI.PropertyField(rect, property.FindPropertyRelative("m_maxAmount"));
			rect.y += fieldHeight + kSpacing;
			EditorGUI.PropertyField(rect, property.FindPropertyRelative("m_isInfinite"));

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
