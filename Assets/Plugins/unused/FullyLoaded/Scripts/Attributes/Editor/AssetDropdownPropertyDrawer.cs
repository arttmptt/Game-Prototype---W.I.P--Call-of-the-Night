using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(AssetDropdownAttribute))]
	public class AssetDropdownAttributePropertyDrawer : PropertyDrawer
	{
		// -------------------------------------------------------------------------------------------------

		private struct PerPropertyData
		{
			// nothing needed currently
		}

		private struct PerAssetTypeData
		{
			public List<ScriptableObject> assets;
		}

		// since property drawer instances can be re-used, we need to key the data by property path or
		// the type we're dealing with where appropriate
		private Dictionary<string, PerPropertyData> m_perPropertyData = new Dictionary<string, PerPropertyData>();
		private Dictionary<System.Type, PerAssetTypeData> m_perAssetTypeData = new Dictionary<System.Type, PerAssetTypeData>();

		// -------------------------------------------------------------------------------------------------

		private void FindAssets(System.Type type, ref List<ScriptableObject> assets)
		{
			if (assets == null)
			{
				assets = new List<ScriptableObject>();
			}
			else
			{
				assets.Clear();
			}

			// find all assets in the project matching the scriptable object type
			string[] guids = AssetDatabase.FindAssets($"t:{type.Name}");
			for (int i = 0; i < guids.Length; ++i)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				if (path.Length > 0)
				{
					ScriptableObject obj = AssetDatabase.LoadAssetAtPath(path, type) as ScriptableObject;
					if (obj != null)
					{
						assets.Add(obj);
					}
				}
			}
		}

		// -------------------------------------------------------------------------------------------------

		private GenericMenu BuildDropdownMenu(SerializedProperty property, PerPropertyData propertyData, PerAssetTypeData assetTypeData)
		{
			ScriptableObject obj = property.objectReferenceValue as ScriptableObject;
			AssetDropdownAttribute attr = attribute as AssetDropdownAttribute;

			GenericMenu menu = new GenericMenu();
			if (attr != null && attr.allowNull)
			{
				menu.AddItem(new GUIContent("None"), obj == null, OnMenuSelection, new KeyValuePair<SerializedProperty, ScriptableObject>(property, null));
				menu.AddSeparator("");
			}
			for (int i = 0; i < assetTypeData.assets.Count; ++i)
			{
				menu.AddItem(new GUIContent(assetTypeData.assets[i].name), obj == assetTypeData.assets[i], OnMenuSelection, new KeyValuePair<SerializedProperty, ScriptableObject>(property, assetTypeData.assets[i]));
			}

			return menu;
		}

		// -------------------------------------------------------------------------------------------------

		private void OnMenuSelection(object userData)
		{
			KeyValuePair<SerializedProperty, ScriptableObject> data = (KeyValuePair<SerializedProperty, ScriptableObject>)userData;
			ScriptableObject current = data.Key.objectReferenceValue as ScriptableObject;
			if (current != data.Value)
			{
				data.Key.objectReferenceValue = data.Value;
				data.Key.serializedObject.ApplyModifiedProperties();
			}
		}

		// -------------------------------------------------------------------------------------------------

		private void GetDataForThisProperty(SerializedProperty property, out PerPropertyData propertyData, out PerAssetTypeData assetTypeData)
		{
			string path = property.propertyPath;

			if (!m_perPropertyData.TryGetValue(path, out propertyData))
			{
				m_perPropertyData.Add(path, propertyData);
			}

			if (!m_perAssetTypeData.TryGetValue(fieldInfo.FieldType, out assetTypeData))
			{
				FindAssets(fieldInfo.FieldType, ref assetTypeData.assets);
				m_perAssetTypeData.Add(fieldInfo.FieldType, assetTypeData);
			}
		}

		// -------------------------------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			PerPropertyData propertyData;
			PerAssetTypeData assetTypeData;
			GetDataForThisProperty(property, out propertyData, out assetTypeData);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			ScriptableObject obj = property.objectReferenceValue as ScriptableObject;
			string text = (obj != null) ? obj.name : "None";
			if (EditorGUI.DropdownButton(position, new GUIContent(text), FocusType.Keyboard))
			{
				BuildDropdownMenu(property, propertyData, assetTypeData).DropDown(position);
			}
		}

		// -------------------------------------------------------------------------------------------------

		public override bool CanCacheInspectorGUI(SerializedProperty property)
		{
			return false;
		}

		// -------------------------------------------------------------------------------------------------

		// this is the UIElements version, used when the property appears in a UIElements editor/inspector
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();

			PerPropertyData propertyData;
			PerAssetTypeData assetTypeData;
			GetDataForThisProperty(property, out propertyData, out assetTypeData);

			AssetDropdownAttribute attr = attribute as AssetDropdownAttribute;

			// create the list of possible choices, including the null choice
			List<ScriptableObject> choices = new List<ScriptableObject>();
			if ((attr != null && attr.allowNull) || assetTypeData.assets.Count == 0)
			{
				choices.Add(null);
			}
			choices.AddRange(assetTypeData.assets);

			// create a popup field
			PopupField<ScriptableObject> popup = new PopupField<ScriptableObject>(property.displayName, choices, 0,
				(obj) =>
				{
					return (obj != null) ? obj.name : "None";
				},
				(obj) =>
				{
					return (obj != null) ? obj.name : "None";
				}
			);

			// register change event callback
			popup.RegisterCallback<ChangeEvent<ScriptableObject>>(evt =>
			{
				property.objectReferenceValue = evt.newValue;
				property.serializedObject.ApplyModifiedProperties();

			// send an event up the hierarchy
			using (SerializedPropertyChangeEvent newEvent = SerializedPropertyChangeEvent.GetPooled(property))
				{
				// find the topmost parent VisualElement
				VisualElement parentElement = root.parent;
					while (true)
					{
						if (parentElement.parent == null)
						{
							break;
						}
						parentElement = parentElement.parent;
					}

					newEvent.target = root;
					parentElement.SendEvent(newEvent);
				}
			});

			// set initial value
			// if the initial value is null, and None is not allowed as specified by the attribute,
			// attempt to set a default value using the first choice in the list
			if ((property.objectReferenceValue as ScriptableObject) == null && attr != null && !attr.allowNull)
			{
				if (choices.Count > 0)
				{
					popup.value = choices[0];
					property.objectReferenceValue = choices[0];
					property.serializedObject.ApplyModifiedProperties();
				}
			}
			else
			{
				popup.value = (property.objectReferenceValue as ScriptableObject);
			}

			root.Add(popup);
			return root;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
