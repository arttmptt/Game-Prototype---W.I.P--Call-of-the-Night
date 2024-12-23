using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	public interface ICustomBehaviourSettingsSpawner<SettingsBaseType>
	{
		public SettingsBaseType CreateBehaviourSettingsInstance();
	}
}
