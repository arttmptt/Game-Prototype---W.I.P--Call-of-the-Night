using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace FullyLoaded
{
    public class FoldoutHeaderElement : CustomVisualElement
    {
        // -------------------------------------------------------------------------------------------------

        private VisualElement m_container = new VisualElement();
        private Image m_foldoutArrow = new Image();

        private bool m_childrenVisible = true;
        private bool m_reparented = false;
        private string m_uniqueId = "";

        private System.Action m_clicked = null;
        private Clickable m_clickable = null;

        public string text { get; set; }
        public bool startsExpanded { get; set; }

        // -------------------------------------------------------------------------------------------------

        public new class UxmlFactory : UxmlFactory<FoldoutHeaderElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_text =
                new UxmlStringAttributeDescription { name = "text" };
            UxmlBoolAttributeDescription m_startsExpanded =
                new UxmlBoolAttributeDescription { name = "starts-expanded", defaultValue = true };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var ate = ve as FoldoutHeaderElement;
                ate.text = m_text.GetValueFromBag(bag, cc);
                ate.startsExpanded = m_startsExpanded.GetValueFromBag(bag, cc);
                ate.Initialize();
            }
        }

        // -------------------------------------------------------------------------------------------------

        // foldout state (visibility) is cached for all foldouts here for a single editor session
        private static Dictionary<string, bool> m_globalFoldoutVisibility = new Dictionary<string, bool>();

        private static bool GetCachedFoldoutVisibility(string id, out bool visible)
        {
            if (!m_globalFoldoutVisibility.ContainsKey(id))
            {
                visible = false;
                return false;
            }

            visible = m_globalFoldoutVisibility[id];
            return true;
        }

        private static void SetCachedFoldoutVisibility(string id, bool visible)
        {
            m_globalFoldoutVisibility[id] = visible;
        }

        // -------------------------------------------------------------------------------------------------

        private void Initialize()
        {
            SetId(text.Replace(' ', '_'));

            // insert label as the first child element
            Label headerLabel = new Label(text);
            VisualElement labelContainer = new VisualElement();
            labelContainer.style.flexDirection = FlexDirection.Row;
            labelContainer.Add(m_foldoutArrow);
            labelContainer.Add(headerLabel);
            Insert(0, labelContainer);

            // add click event handling to the label container
            m_clicked = ToggleVisibility;
            m_clickable = new Clickable(m_clicked);
            labelContainer.AddManipulator(m_clickable);

            // set class names for styling
            AddToClassList("FoldoutRoot");
            labelContainer.AddToClassList("FoldoutHeaderContainer");
            headerLabel.AddToClassList("FoldoutHeader");
            m_foldoutArrow.AddToClassList("FoldoutArrow");
            m_container.AddToClassList("FoldoutContentsContainer");

            // register for the AttachToPanel event, this is a good place to reparent the child
            // elements
            RegisterCallback<AttachToPanelEvent>(evt =>
            {
                OnAttachedToPanel(evt);
            });
        }

        // -------------------------------------------------------------------------------------------------

        private void OnAttachedToPanel(AttachToPanelEvent evt)
        {
            if (!m_reparented)
            {
                // reparent all children (except the label) so that they are children of m_container instead
                // then m_container becomes the only other child
                List<VisualElement> childList = new List<VisualElement>();
                childList.AddRange(Children());
                for (int i = 1; i < childList.Count; ++i)
                {
                    m_container.Add(childList[i]);
                }
                Add(m_container);
                m_reparented = true;

                // construct a unique id for this element, by concatenating the id's of all
                // CustomVisualElements above it in the hierarchy
                List<CustomVisualElement> elements = new List<CustomVisualElement>();
                VisualElement element = this;
                while (true)
                {
                    CustomVisualElement customElement = element as CustomVisualElement;
                    if (customElement != null)
                    {
                        elements.Add(customElement);
                    }

                    if (element.parent == null)
                    {
                        break;
                    }
                    element = element.parent;
                }
                for (int i = elements.Count - 1; i >= 0; --i)
                {
                    m_uniqueId += elements[i].id;
                    if (i > 0)
                    {
                        m_uniqueId += ".";
                    }
                }

                // restore foldout visibility state if possible
                bool cachedVisibility = true;
                if (GetCachedFoldoutVisibility(m_uniqueId, out cachedVisibility))
                {
                    SetVisibility(cachedVisibility);
                }
                else
                {
                    SetCachedFoldoutVisibility(m_uniqueId, startsExpanded);
                }
            }
        }

        // -------------------------------------------------------------------------------------------------

        private void SetVisibility(bool visible)
        {
            m_childrenVisible = visible;
            m_container.style.display = (m_childrenVisible) ? DisplayStyle.Flex : DisplayStyle.None;

            if (visible)
            {
                m_foldoutArrow.RemoveFromClassList("FoldoutArrowCollapsed");
                m_foldoutArrow.AddToClassList("FoldoutArrowExpanded");
            }
            else
            {
                m_foldoutArrow.RemoveFromClassList("FoldoutArrowExpanded");
                m_foldoutArrow.AddToClassList("FoldoutArrowCollapsed");
            }
        }

        // -------------------------------------------------------------------------------------------------

        private void ToggleVisibility()
        {
            SetVisibility(!m_childrenVisible);
            SetCachedFoldoutVisibility(m_uniqueId, m_childrenVisible);
        }

        // -------------------------------------------------------------------------------------------------
    }
}
