// <copyright file="DrakkarItemInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.IO;

using UnityEngine;
using UnityEditor;

namespace V4F.Game.Management
{

    [CustomEditor(typeof(DrakkarItem)), InitializeOnLoad]
    public class DrakkarItemInspector : Editor
    {
        #region Fields
        private static readonly GUIContent[] __content = null;
        private DrakkarItem _self = null;
        #endregion

        #region Constructors
        static DrakkarItemInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent("Transition"),
                new GUIContent("Next state"),
                new GUIContent("Unlocked"),
                new GUIContent("Load scene"),                
            };
        }
        #endregion

        #region Methods
        public override void OnInspectorGUI()
        {            
            serializedObject.Update();

            var propTransition = serializedObject.FindProperty("_transition");
            EditorGUILayout.PropertyField(propTransition, __content[0], false);

            var propState = serializedObject.FindProperty("_state");
            EditorGUILayout.PropertyField(propState, __content[1], false);

            var propUnlocked = serializedObject.FindProperty("_unlocked");
            EditorGUILayout.PropertyField(propUnlocked, __content[2], false);

            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(_self.scenePath);
            EditorGUI.BeginChangeCheck();
            var newScene = EditorGUILayout.ObjectField(__content[3], oldScene, typeof(SceneAsset), false) as SceneAsset;
            if (EditorGUI.EndChangeCheck())
            {                
                var path = AssetDatabase.GetAssetPath(newScene);
                serializedObject.FindProperty("_scenePath").stringValue = path;
                var name = Path.GetFileNameWithoutExtension(path);
                serializedObject.FindProperty("_sceneName").stringValue = name;
            }            

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorApplication.RepaintHierarchyWindow();
                EditorUtility.SetDirty(_self);
            }
        }

        private void OnEnable()
        {
            _self = target as DrakkarItem;
        }
        #endregion
    }

}
