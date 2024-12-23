using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullyLoaded
{
    public static class LayerUtils
    {
        // -------------------------------------------------------------------------------------------------

        private static List<int> m_validLayers = new List<int>();

        // -------------------------------------------------------------------------------------------------

        private static void FindValidLayers()
        {
            for (int i = 0; i < 32; ++i)
            {
                // layers that have not been set up in the project settings (blank name) default to returning
                // false from GetIgnoreLayerCollision, don't add them to the list of valid layers
                if (LayerMask.LayerToName(i).Length != 0)
                {
                    m_validLayers.Add(i);
                }
            }
        }

        // -------------------------------------------------------------------------------------------------

        // returns a layermask bitfield representing all layers that are enabled for collision with sourceLayer
        // as set in the physics project settings
        public static int GetLayerCollisionMask(int sourceLayer)
        {
            if (m_validLayers.Count == 0)
            {
                FindValidLayers();
            }

            int mask = 0;

            if (sourceLayer >= 0 && sourceLayer < 32)
            {
                int layer = 0;
                for (int i = 0; i < m_validLayers.Count; ++i)
                {
                    // check which other layers are colliding with the source layer
                    layer = m_validLayers[i];
                    if (Physics.GetIgnoreLayerCollision(sourceLayer, layer) == false)
                    {
                        mask += (1 << layer);
                    }
                }
            }

            return mask;
        }

        // -------------------------------------------------------------------------------------------------
    }
}
