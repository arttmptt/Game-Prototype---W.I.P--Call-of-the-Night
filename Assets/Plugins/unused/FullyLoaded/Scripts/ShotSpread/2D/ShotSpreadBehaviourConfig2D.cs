using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class ShotSpreadBehaviourConfig2D : CustomBehaviourConfig<ShotSpreadBehaviourAsset2D, ShotSpreadBehaviourSettings2D>
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField, AssetDropdown(allowNull = true)]
		private ShotSpreadBehaviourAsset2D m_behaviour = null;

		public override ShotSpreadBehaviourAsset2D behaviour { get { return m_behaviour; } }

		// -------------------------------------------------------------------------------------------------

		public override string GetDisplayName()
		{
			return "Shot-Spread Config";
		}

		// -------------------------------------------------------------------------------------------------
	}
}
