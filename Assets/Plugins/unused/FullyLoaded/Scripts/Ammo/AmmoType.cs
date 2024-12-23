using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[CreateAssetMenu(menuName = "Fully Loaded/Ammo Type Asset")]
	public class AmmoType : UniqueScriptableObject
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private string m_ammoName = "";

		public string ammoName { get { return m_ammoName; } }

		// -------------------------------------------------------------------------------------------------
	}
}
