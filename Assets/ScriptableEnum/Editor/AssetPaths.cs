using ScriptableEnumSystem.CommonDS;
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace ScriptableEnumSystem.EditorHandles
{
    public class ASSET_PATHS 
    {
        

        public static SingleSearchField<string> ScriptableEnumContainerAssetPath = new SingleSearchField<string>(() =>
        {
            string[] guids = AssetDatabase.FindAssets("t:ScriptableEnumsContainer");
            foreach (string item in guids)
            {
                ScriptableEnumsContainer scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableEnumsContainer>(AssetDatabase.GUIDToAssetPath(item));
                if (scriptableObject != null)
                    return AssetDatabase.GUIDToAssetPath(item);
            }

            return "Assets/";
        });

        //public static SingleSearchField<string> MethodArgumentDrawControlAssetPath = new SingleSearchField<string>(() =>
        //{
        //    string[] guids = AssetDatabase.FindAssets("t:"+nameof(MethodArgumentDrawControlSO));
        //    foreach (string item in guids)
        //    {
        //        MethodArgumentDrawControlSO scriptableObject = AssetDatabase.LoadAssetAtPath<MethodArgumentDrawControlSO>(AssetDatabase.GUIDToAssetPath(item));
        //        if (scriptableObject != null)
        //            return AssetDatabase.GUIDToAssetPath(item);
        //    }

        //    throw new System.Exception("Failed to resolve path for MethodArgumentDrawControlSOLocation");
        //});


    }
}
