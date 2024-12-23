using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class ShotSpreadBehaviourConfig : CustomBehaviourConfig<ShotSpreadBehaviourAsset, ShotSpreadBehaviourSettings>
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField, AssetDropdown(allowNull = true)]
		private ShotSpreadBehaviourAsset m_behaviour = null;

		public override ShotSpreadBehaviourAsset behaviour { get { return m_behaviour; } }

		// -------------------------------------------------------------------------------------------------

		public override string GetDisplayName()
		{
			return "Shot-Spread Config";
		}

		// -------------------------------------------------------------------------------------------------
	}
}
