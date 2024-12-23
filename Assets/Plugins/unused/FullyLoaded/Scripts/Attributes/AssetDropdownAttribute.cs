using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
	public class AssetDropdownAttribute : PropertyAttribute
	{
		// -------------------------------------------------------------------------------------------------

		private bool m_allowNull = true;

		// -------------------------------------------------------------------------------------------------

		public bool allowNull
		{
			get { return m_allowNull; }
			set { m_allowNull = value; }
		}

		// -------------------------------------------------------------------------------------------------
	}
}
