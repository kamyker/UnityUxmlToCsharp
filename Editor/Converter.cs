using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace KS.UxmlToCsharp
{
    static class Converter
    {
        [MenuItem("Assets/Uxml To C#/Create C# class", true)]
        static bool ValidateCreate()
        {
            return Selection.activeObject.GetType() == typeof(VisualTreeAsset);
        }

        [MenuItem("Assets/Uxml To C#/Create C# class")]
        static void Create()
        {
            CreateOrUpdateClass(Selection.activeObject as VisualTreeAsset);
        }

        [MenuItem("Assets/Uxml To C#/Update C# class", true)]
        static bool ValidateUpdateClass()
        {
            return Selection.activeObject.GetType() == typeof(MonoScript);
        }

        [MenuItem("Assets/Uxml To C#/Update C# class")]
        static void UpdateClass()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            var script = (MonoScript)Selection.activeObject;
            object classInstance = Activator.CreateInstance(script.GetClass());
            string guid = (string)classInstance.GetType().
                GetProperty("uxmlGuid", BindingFlags.NonPublic | BindingFlags.Instance).
                GetValue(classInstance);

            CreateOrUpdateClass(AssetDatabase.LoadAssetAtPath(
                AssetDatabase.GUIDToAssetPath(guid), typeof(VisualTreeAsset)) as VisualTreeAsset);
        }

        public static void CreateOrUpdateClass(VisualTreeAsset uxml)
        {
            if (uxml == null)
            {
                Debug.LogError("Uxml is null");
                return;
            }

            var path = AssetDatabase.GetAssetPath(uxml);
            var template = uxml.CloneTree();
            var sb = new StringBuilder();
            sb.AppendLine("using KS.UxmlToCsharp;");
            sb.AppendLine("using UnityEngine.UIElements;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"public class {uxml.name}Converted : UxmlConvertedBase");
            sb.AppendLine("{");
            sb.AppendLine($"    protected override string uxmlGuid => \"{AssetDatabase.AssetPathToGUID(path)}\";");

            //key name, value type string
            Dictionary<string, string> fields = new Dictionary<string, string>();
            List<string> templates = new List<string>();
            foreach (var e in template.Children())
                RecursiveFill(e, ref fields, ref templates);

            foreach (var field in fields)
                sb.AppendLine($"    public {field.Value} {field.Key};");

            sb.AppendLine($"    protected override void AssignFields()");
            sb.AppendLine("    {");
            foreach (var field in fields)
                sb.AppendLine($"        {field.Key} = ({field.Value})elementsToAssign[\"{field.Key}\"];");
            sb.AppendLine("    }");

            if (templates.Count > 0)
            {
                sb.AppendLine($"    protected override void MakeTemplatesInstances(List<VisualElement> markedToDelete)");
                sb.AppendLine("    {");
                for (int i = 0; i < templates.Count; i++)
                    sb.AppendLine($"        markedToDelete[{i}].ReplaceElementWithChildrenOf(new {templates[i]}());");
                sb.AppendLine("    }");
            }

            sb.AppendLine("}");
            var pathCs = path.Replace(".uxml", "Converted.cs");
            File.WriteAllText(pathCs, sb.ToString());
            AssetDatabase.Refresh();
        }

        static void RecursiveFill(VisualElement element, ref Dictionary<string, string> fields, ref List<string> templates)
        {
            if (element.customStyle.TryGetValue(new CustomStyleProperty<string>("--csName"), out string name))
            {
                if (fields.ContainsKey(name))
                    Debug.LogError($"UXML has multiple elements with \"{name}\" --csName, only first one will be referenced");
                else
                    fields.Add(name, element.GetType().Name);
            }

            if (element.customStyle.TryGetValue(new CustomStyleProperty<string>("--csTemplate"), out string template))
                templates.Add(template);

            foreach (var item in element.Children())
                RecursiveFill(item, ref fields, ref templates);
        }
    }
}