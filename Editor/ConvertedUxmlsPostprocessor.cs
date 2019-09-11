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

    class ConvertedUxmlsPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                if (Path.GetExtension(str).IndexOf("uxml", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    var pathCs = str.Replace(".uxml", "Converted.cs");
                    if (File.Exists(pathCs))
                    {
                        Debug.Log($"Updating: {pathCs}");
                        Converter.CreateOrUpdateClass(AssetDatabase.
                            LoadAssetAtPath(str, typeof(VisualTreeAsset)) as VisualTreeAsset);
                    }
                }
            }
        }
    }
}