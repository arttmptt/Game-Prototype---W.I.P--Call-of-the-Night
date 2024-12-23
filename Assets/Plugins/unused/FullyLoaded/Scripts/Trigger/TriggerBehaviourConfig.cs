using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class TriggerBehaviourConfig : CustomBehaviourConfig<TriggerBehaviourAsset, TriggerBehaviourSettings>
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField, AssetDropdown(allowNull = false)]
		private TriggerBehaviourAsset m_behaviour = null;

		public override TriggerBehaviourAsset behaviour { get { return m_behaviour; } }

		// -------------------------------------------------------------------------------------------------

		public override string GetDisplayName()
		{
			return "Trigger Config";
		}

		// -------------------------------------------------------------------------------------------------
	}
}
