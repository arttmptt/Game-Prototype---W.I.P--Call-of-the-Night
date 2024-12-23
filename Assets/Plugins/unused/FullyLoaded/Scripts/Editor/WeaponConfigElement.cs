using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
	public class WeaponConfigElement : BindableElement
	{
		// -------------------------------------------------------------------------------------------------

		public WeaponConfigElement(EWeaponMode weaponMode, WeaponConfig target, SerializedProperty property)
		{
			bindingPath = property.propertyPath;

			// import uxml
			VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/FullyLoaded/Scripts/Editor/WeaponConfigElement.uxml");
			uxml.CloneTree(this);

			// manually create the trigger config element
			VisualElement triggerConfig = this.Query<VisualElement>("TriggerConfig");
			if (triggerConfig != null)
			{
				SerializedProperty triggerConfigProp = property.FindPropertyRelative("m_triggerConfig");
				triggerConfig.Add(new CustomBehaviourConfigElement<TriggerBehaviourAsset, TriggerBehaviourSettings>(target.triggerConfig, triggerConfigProp));
			}
			
			// manually create the shot-spread config element, depends on if we're in 2d or 3d mode
			VisualElement shotSpreadConfig = this.Query<VisualElement>("ShotSpreadConfig");
			if (shotSpreadConfig != null)
			{
				#if FULLY_LOADED_2D
				SerializedProperty shotSpreadConfigProp = property.FindPropertyRelative("m_shotSpreadConfig2D");
				shotSpreadConfig.Add(new CustomBehaviourConfigElement<ShotSpreadBehaviourAsset2D, ShotSpreadBehaviourSettings2D>(target.shotSpreadConfig2D, shotSpreadConfigProp));
				#else
				SerializedProperty shotSpreadConfigProp = property.FindPropertyRelative("m_shotSpreadConfig");
				shotSpreadConfig.Add(new CustomBehaviourConfigElement<ShotSpreadBehaviourAsset, ShotSpreadBehaviourSettings>(target.shotSpreadConfig, shotSpreadConfigProp));
				#endif
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
