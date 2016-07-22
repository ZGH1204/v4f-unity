// <copyright file="ScriptableHelper.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.IO;

using UnityEngine;
using UnityEditor;

namespace V4F
{

    public static class ScriptableHelper
    {
        public static void CreateAsset<T>() where T : ScriptableObject
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            
            var pathAndName = string.Format("{0}/New{1}.asset", path, typeof(T).Name);
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(pathAndName);

            T asset = ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }


    }
	
}
