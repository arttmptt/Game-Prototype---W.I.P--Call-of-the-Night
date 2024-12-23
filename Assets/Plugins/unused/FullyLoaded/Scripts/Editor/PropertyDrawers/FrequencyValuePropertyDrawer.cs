using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(FrequencyValue))]
	public class FrequencyValuePropertyDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();
			root.AddToClassList("FrequencyValueContainer");

			VisualElement row = new VisualElement();
			VisualElement left = new VisualElement();
			VisualElement right = new VisualElement();
			VisualElement upperLeft = new VisualElement();
			VisualElement upperRight = new VisualElement();
			VisualElement lowerLeft = new VisualElement();
			VisualElement lowerRight = new VisualElement();

			left.style.flexGrow = 1;
			right.style.flexGrow = 1;
			upperLeft.style.flexGrow = 1;
			upperRight.style.flexGrow = 1;
			lowerLeft.style.flexGrow = 1;
			lowerRight.style.flexGrow = 1;

			row.style.flexDirection = FlexDirection.Row;

			// heading label
			Label header = new Label(property.displayName);
			header.AddToClassList("FrequencyValueHeader");

			Label freqLabel = new Label("Per Second");
			Label intervalLabel = new Label("Interval");
			upperLeft.Add(freqLabel);
			upperRight.Add(intervalLabel);

			FloatField frequencyField = null;
			SerializedProperty frequencyProperty = property.FindPropertyRelative("m_frequency");
			if (frequencyProperty != null)
			{
				frequencyField = new FloatField();
				frequencyField.BindProperty(frequencyProperty);
				lowerLeft.Add(frequencyField);
			}

			FloatField intervalField = null;
			SerializedProperty intervalProperty = property.FindPropertyRelative("m_interval");
			if (intervalProperty != null)
			{
				intervalField = new FloatField();
				intervalField.BindProperty(intervalProperty);
				lowerRight.Add(intervalField);
			}

			// register for value changed callbacks, when one value changes update the other
			if (frequencyField != null && intervalField != null)
			{
				frequencyField.RegisterValueChangedCallback(ctx =>
				{
					if (ctx.newValue > 0.0f)
					{
						intervalField.value = 1.0f / ctx.newValue;
					}
				});

				intervalField.RegisterValueChangedCallback(ctx =>
				{
					if (ctx.newValue > 0.0f)
					{
						frequencyField.value = 1.0f / ctx.newValue;
					}
				});
			}

			// build the visual asset tree
			root.Add(header);
			root.Add(row);
			row.Add(left);
			row.Add(right);
			left.Add(upperLeft);
			left.Add(lowerLeft);
			right.Add(upperRight);
			right.Add(lowerRight);

			return root;
		}
	}
}
