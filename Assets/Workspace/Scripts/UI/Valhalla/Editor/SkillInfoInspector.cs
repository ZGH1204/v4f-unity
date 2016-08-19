// <copyright file="SkillInfoInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEditor;

namespace V4F.UI.Valhalla
{

    [CustomEditor(typeof(SkillInfo)), InitializeOnLoad]
    public class SkillInfoInspector : Editor
    {
        private static readonly GUIContent[] __content = null;        
        private SkillInfo _self;

        static SkillInfoInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent("Statistics"),                
                new GUIContent("Value UI"),                
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var statsProp = serializedObject.FindProperty("_stats");
            EditorGUILayout.PropertyField(statsProp, __content[0]);            

            var valueUIProp = serializedObject.FindProperty("_valueUI");
            EditorGUILayout.PropertyField(valueUIProp, __content[1]);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_self);
            }
        }        

        private void OnEnable()
        {
            _self = target as SkillInfo;
        }
    }

}
