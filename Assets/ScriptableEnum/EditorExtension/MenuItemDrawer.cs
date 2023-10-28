using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


namespace ScriptableEnumSystem.EditorHandles
{

    public class MenuBarItems : Editor 
    {
        [MenuItem(("ScriptableEnumSystem/ScriptableEnumContainer"))]
        public static void CreateIdContainer()
        {
            const string ASSET_PATH = ASSET_PATHS.ScriptableEnumContainerAssetPath;
            ValidateFolderHierarchy();
            CreateSOIfNotPresent(ASSET_PATH);


            static void ValidateFolderHierarchy()
            {
                //split asset path
                string assetPath = ASSET_PATH;
                List<string> assets = assetPath.Split("/".ToCharArray()).ToList();
                if (assetPath.Contains(".asset"))
                {
                    assets.RemoveAt(assets.Count - 1);
                }

                //create folder paths based on folder names
                string cache = string.Empty;
                List<string> folderHierarchyPaths = new List<string>();
                for (int i = 0; i < assets.Count; i++)
                {
                    string conditionalForwardSlash = (i == 0) ? string.Empty : "/";
                    cache += $"{conditionalForwardSlash}{assets[i]}";
                    folderHierarchyPaths.Add(cache);
                }

                //create folders
                for (int i = 0; i < folderHierarchyPaths.Count; i++)
                {
                    if (!AssetDatabase.IsValidFolder(folderHierarchyPaths[i]))
                    {
                        AssetDatabase.CreateFolder(folderHierarchyPaths[i - 1], assets[i]);
                    }
                }

                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();

            }


            static void CreateSOIfNotPresent(string ASSET_PATH)
            {
                ScriptableEnumsContainer dataSo = AssetDatabase.LoadAssetAtPath<ScriptableEnumsContainer>(ASSET_PATH);
                if (dataSo == null)
                {
                    dataSo = ScriptableObject.CreateInstance<ScriptableEnumsContainer>();
                    AssetDatabase.CreateAsset(dataSo, ASSET_PATH);
                    return;
                }
            }

        }


    }
}
