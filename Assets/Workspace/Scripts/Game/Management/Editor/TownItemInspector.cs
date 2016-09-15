// <copyright file="TownItemInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEditor;

namespace V4F.Game.Management
{

    [CustomEditor(typeof(TownItem)), InitializeOnLoad]
    public class TownItemInspector : Editor
    {
        #region Fields
        private static readonly GUIContent[] __content = null;
        private TownItem _self = null;
        #endregion

        #region Constructors
        static TownItemInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent("Transition"),
                new GUIContent("Next state"),
                new GUIContent("Unlocked"),
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

            if (GUI.changed)
            {                
                serializedObject.ApplyModifiedProperties();
                EditorApplication.RepaintHierarchyWindow();
                EditorUtility.SetDirty(_self);
            }
        }

        private void OnEnable()
        {
            _self = target as TownItem;
        }
        #endregion
    }

}
