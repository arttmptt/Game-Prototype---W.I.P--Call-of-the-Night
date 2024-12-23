using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	[System.Serializable]
	public class WeaponConfig
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private TriggerBehaviourConfig m_triggerConfig = new TriggerBehaviourConfig();
		[SerializeField] private AmmoConfig m_ammoConfig = new AmmoConfig();
		[SerializeField] private BurstConfig m_burstConfig = new BurstConfig();
		[SerializeField] private MultiShotConfig m_multiShotConfig = new MultiShotConfig();
		[SerializeField] private ShotSpreadBehaviourConfig m_shotSpreadConfig = new ShotSpreadBehaviourConfig();
		[SerializeField] private ShotSpreadBehaviourConfig2D m_shotSpreadConfig2D = new ShotSpreadBehaviourConfig2D();
		[SerializeField] private ProjectileDefinition m_projectile = null;

		public TriggerBehaviourConfig triggerConfig           { get { return m_triggerConfig; } }
		public AmmoConfig ammoConfig                          { get { return m_ammoConfig; } }
		public BurstConfig burstConfig                        { get { return m_burstConfig; } }
		public MultiShotConfig multiShotConfig                { get { return m_multiShotConfig; } }
		public ShotSpreadBehaviourConfig shotSpreadConfig     { get { return m_shotSpreadConfig; } }
		public ShotSpreadBehaviourConfig2D shotSpreadConfig2D { get { return m_shotSpreadConfig2D; } }
		public ProjectileDefinition projectile                { get { return m_projectile; } }

		// -------------------------------------------------------------------------------------------------
	}
}
