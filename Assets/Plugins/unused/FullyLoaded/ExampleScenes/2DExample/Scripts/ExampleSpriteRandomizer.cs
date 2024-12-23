using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public class ExampleSpriteRandomizer : MonoBehaviour
    {
        // -------------------------------------------------------------------------------------------------

        [SerializeField] private List<Sprite> m_sprites = new List<Sprite>();

        private SpriteRenderer m_renderer = null;

		// -------------------------------------------------------------------------------------------------

		private void Awake()
		{
			m_renderer = GetComponent<SpriteRenderer>();

			for (int i = m_sprites.Count - 1; i >= 0; --i)
			{
				if (m_sprites[i] == null)
				{
					m_sprites.RemoveAt(i);
				}
			}
		}

		// -------------------------------------------------------------------------------------------------

		public void Randomize()
		{
			if (m_renderer != null && m_sprites.Count > 0)
			{
				m_renderer.sprite = m_sprites[Random.Range(0, m_sprites.Count)];
			}
		}

		// -------------------------------------------------------------------------------------------------
	}
}
