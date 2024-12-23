using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FullyLoaded
{
	public class UI_CheckDefine : MonoBehaviour
	{
		// -------------------------------------------------------------------------------------------------

		[SerializeField] private bool m_expects2D = false;

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			Text text = GetComponentInChildren<Text>();
			if (text != null)
			{
				text.text = "";

				bool is2D = false;
				#if FULLY_LOADED_2D
				is2D = true;
				#endif

				// display a warning if the 2d/3d mode is set incorrectly for the given scene
				if (m_expects2D && !is2D)
				{
					text.text = "You need to add the FULLY_LOADED_2D scripting define for 2D mode!";
				}
				else if (!m_expects2D && is2D)
				{
					text.text = "You need to remove the FULLY_LOADED_2D scripting define for 3D mode!";
				}
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
