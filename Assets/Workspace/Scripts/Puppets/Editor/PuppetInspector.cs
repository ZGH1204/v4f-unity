// <copyright file="PuppetInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;

namespace V4F.Puppets
{

    [CustomEditor(typeof(Puppet)), InitializeOnLoad]
    public class PuppetInspector : Editor
    {
        #region Fields        
        //private static readonly GUIContent[] __content = null;        

        private Puppet _self = null;
        #endregion

        #region Constructors
        static PuppetInspector()
        {
            /*
            __content = new GUIContent[]
            {
                new GUIContent("Specification:"),
                new GUIContent("Skill set:"),
                new GUIContent("Name:"),
                new GUIContent("Prefab:"),
                new GUIContent("Icon:"),
            };
            */            
        }
        #endregion

        #region Methods
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox(_self.uniqueID, MessageType.None);

            /*
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[0], CustomStyles.italicLabel);
            var spec = serializedObject.FindProperty("_spec");
            spec.objectReferenceValue = EditorGUILayout.ObjectField(spec.objectReferenceValue, typeof(PuppetSpec), false) as PuppetSpec;

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[1], CustomStyles.italicLabel);
            var skillSet = serializedObject.FindProperty("_skillSet");
            skillSet.objectReferenceValue = EditorGUILayout.ObjectField(skillSet.objectReferenceValue, typeof(PuppetSkillSet), false) as PuppetSkillSet;

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[2], CustomStyles.italicLabel);
            var nameProp = serializedObject.FindProperty("_name");
            var name = Regex.Replace(EditorGUILayout.DelayedTextField(nameProp.stringValue), @"[^a-zA-Z0-9_ ]", "").Trim();
            if (!string.IsNullOrEmpty(name) && (name.Length > 3))
            {
                nameProp.stringValue = name;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[3], CustomStyles.italicLabel);
            var prefabProp = serializedObject.FindProperty("_prefab");
            prefabProp.objectReferenceValue = EditorGUILayout.ObjectField(GUIContent.none, prefabProp.objectReferenceValue, typeof(GameObject), false) as GameObject;

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[4], CustomStyles.italicLabel);
            var smallIconProp = serializedObject.FindProperty("_icon");
            smallIconProp.objectReferenceValue = EditorGUILayout.ObjectField(GUIContent.none, smallIconProp.objectReferenceValue, typeof(Sprite), false) as Sprite;

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            */

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_self);
            }
        }

        private void OnEnable()
        {
            _self = target as Puppet;
        }

        private void OnDisable()
        {

        }
        #endregion
    }

}
