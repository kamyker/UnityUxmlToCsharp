using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace KS.UxmlToCsharp
{
    public static class VisualElementExtensions
    {
        public static void AddChildrenOf<T>(this VisualElement el, T withChildren) where T : UxmlConvertedBase
        {
            foreach (var child in withChildren.Root.Children().ToList())
                el.Add(child);
        }

        public static void ReplaceElementWithChildrenOf<T>(this VisualElement el, T withChildren) where T : UxmlConvertedBase
        {
            int templateIndex = el.parent.IndexOf(el);
            VisualElement parent = el.parent;
            var children = withChildren.Root.Children().ToList();
            for (int i = 0; i < children.Count; i++)
                parent.Insert(templateIndex + 1 + i, children[i]);

            parent.RemoveAt(templateIndex);
        }
    }
}