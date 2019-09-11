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

    public abstract class UxmlConvertedBase
    {
        protected virtual string uxmlGuid => "";

        protected Dictionary<string, VisualElement> elementsToAssign = new Dictionary<string, VisualElement>();
        public VisualElement Root = new VisualElement();

        public UxmlConvertedBase()
        {
            var template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(uxmlGuid));
            template.CloneTree(Root);
            foreach (var e in Root.Children())
                RecursiveFromStyle(e, ref elementsToAssign);

            AssignFields();
        }

        protected abstract void AssignFields();

        public static void RecursiveFromStyle(VisualElement element, ref Dictionary<string, VisualElement> fields)
        {
            if (element.customStyle.TryGetValue(new CustomStyleProperty<string>("--csName"), out string name))
            {
                if (fields.ContainsKey(name))
                    Debug.LogError($"UXML has multiple elements with \"{name}\" --csName, only first one will be referenced");
                else
                    fields.Add(name, element);
            }
            foreach (var item in element.Children())
                RecursiveFromStyle(item, ref fields);
        }
    }
}