using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UniqueId = FullyLoaded.UniqueScriptableObject.UniqueId;

namespace FullyLoaded
{
	[CustomPropertyDrawer(typeof(UniqueId))]
	public class UniqueIdPropertyDrawer : PropertyDrawer
	{
		// -------------------------------------------------------------------------------------------------

		private bool m_initialValidateDone = false;

		// -------------------------------------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new VisualElement();
			root.AddToClassList("UniqueId");

			SerializedProperty idProperty = property.FindPropertyRelative("m_uniqueId");
			TextField idTextField = new TextField("Unique Id", 32, false, false, '*');
			idTextField.isDelayed = true;
			idTextField.BindProperty(idProperty);
			root.Add(idTextField);

			idTextField.RegisterValueChangedCallback(evt =>
			{
				// validate the id is unique
				string id = evt.newValue;
				if (!ValidateId(ref id, idProperty))
				{
					// update the property to have the new id
					property.serializedObject.Update();
					idProperty.stringValue = id;
					property.serializedObject.ApplyModifiedProperties();

					// rebind the text field
					idTextField.BindProperty(idProperty);
				}
			});

			// also validate the id the first time the object is selected
			string id = idProperty.stringValue;
			if (!ValidateId(ref id, idProperty))
			{
				// update the property to have the new id
				property.serializedObject.Update();
				idProperty.stringValue = id;
				property.serializedObject.ApplyModifiedProperties();

				// rebind the text field
				idTextField.BindProperty(idProperty);
			}

			return root;
		}

		// -------------------------------------------------------------------------------------------------

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			EditorGUI.BeginChangeCheck();

			position = EditorGUI.PrefixLabel(position, new GUIContent("Unique Id"));

			// create the text field
			SerializedProperty idProperty = property.FindPropertyRelative("m_uniqueId");
			EditorGUI.DelayedTextField(position, idProperty, GUIContent.none);

			// check for edits, blank text, or an object duplication
			if (EditorGUI.EndChangeCheck() || idProperty.stringValue.Length == 0 || !m_initialValidateDone)
			{
				m_initialValidateDone = true;

				string id = idProperty.stringValue;
				if (!ValidateId(ref id, idProperty))
				{
					idProperty.stringValue = id;
				}
			}

			EditorGUI.EndProperty();
		}

		// -------------------------------------------------------------------------------------------------

		// a return value of false means that validation failed and a new id will be set in proposedId.
		// this means the property must then be updated with that new value
		private static bool ValidateId(ref string proposedId, SerializedProperty idProperty)
		{
			if (idProperty.serializedObject.targetObject == null)
			{
				return true;
			}

			if ((idProperty.serializedObject.targetObject as UniqueScriptableObject) == null)
			{
				Debug.LogWarning("UniqueId is only intended to be used with UniqueScriptableObjects");
				return true;
			}

			// cache the details of this particular asset
			System.Type type = idProperty.serializedObject.targetObject.GetType();
			string typename = type.ToString();
			string shortTypename = type.Name;
			string assetPath = AssetDatabase.GetAssetPath(idProperty.serializedObject.targetObject.GetInstanceID());
			string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);

			// find all assets of this type and cache their id's
			string[] guids = AssetDatabase.FindAssets($"t:{typename}");
			List<string> existingIds = new List<string>();
			for (int i = 0; i < guids.Length; ++i)
			{
				if (guids[i] != assetGuid && guids[i].Length > 0)
				{
					Object asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), type);
					UniqueScriptableObject obj = asset as UniqueScriptableObject;
					if (obj != null && obj.uniqueId.Length > 0)
					{
						existingIds.Add(obj.uniqueId);
					}
				}
			}

			// check if the proposed id conflicts with any existing assets
			bool conflict = false;
			for (int i = 0; i < existingIds.Count; ++i)
			{
				if (string.CompareOrdinal(existingIds[i], proposedId) == 0)
				{
					conflict = true;
					Debug.LogWarning($"the unique id \"{proposedId}\" is already in use!\n");
					break;
				}
			}

			// if there is a conflict, pick a new unique name instead
			if (conflict || proposedId.Length == 0)
			{
				int index = 1;
				conflict = true;

				while (conflict)
				{
					proposedId = $"{shortTypename}_{index}";

					conflict = false;
					for (int i = 0; i < existingIds.Count; ++i)
					{
						if (string.CompareOrdinal(existingIds[i], proposedId) == 0)
						{
							conflict = true;
							index++;
							break;
						}
					}
				}

				// validation failed and the id was updated
				return false;
			}

			return true;
		}

		// -------------------------------------------------------------------------------------------------
	}
}
