using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FullyLoaded
{
    public class CustomVisualElement : VisualElement
    {
        // -------------------------------------------------------------------------------------------------

        private string m_id = "";
        public string id { get { return m_id; } }

        // -------------------------------------------------------------------------------------------------

        public void SetId(string newId)
        {
            m_id = newId;
        }

        // -------------------------------------------------------------------------------------------------
    }
}
