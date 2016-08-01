// <copyright file="PuppetDiseaseInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;

namespace V4F.Puppets
{

    [CustomEditor(typeof(PuppetDisease)), InitializeOnLoad]
    public class PuppetDiseaseInspector : Editor
    {
        #region Fields        
        private static readonly GUIContent[] __content = null;

        private PuppetDisease _self;
        #endregion

        #region Constructors
        static PuppetDiseaseInspector()
        {            
            __content = new GUIContent[]
            {
                new GUIContent("Name:"),
                new GUIContent("Description:"),
                new GUIContent("Modifiers"),
                new GUIContent("Health Points (%):"),
                new GUIContent("Accuracy (%):"),
                new GUIContent("Initiative (%):"),
                new GUIContent("Stamina (%):"),
                new GUIContent("Damage (%):"),
            };
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Personage/Disease", false, 802)]
        private static void CreateAsset()
        {
            var asset = ScriptableHelper.CreateAsset<PuppetDisease>();
            Debug.LogFormat("Create \"{0}\" object (ID: {1})", typeof(PuppetDisease).Name, asset.uniqueID);
        }

        public override void OnInspectorGUI()
        {            
            serializedObject.Update();

            EditorGUILayout.HelpBox(_self.uniqueID, MessageType.None);

            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__content[0], CustomStyles.italicLabel);
            var titleProp = serializedObject.FindProperty("_title");
            var selectedIndex = Localization.GetKeyIndex(titleProp.stringValue);
            var select = EditorGUILayout.Popup(selectedIndex, Localization.keys);
            if (select != selectedIndex)
            {
                titleProp.stringValue = Localization.GetKey(select);
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[1], CustomStyles.italicLabel);
            var descProp = serializedObject.FindProperty("_description");
            descProp.stringValue = EditorGUILayout.DelayedTextField(descProp.stringValue).Trim();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[2], EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var modifiersProp = serializedObject.FindProperty("_modifiers");

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[3], CustomStyles.italicLabel);
            var prop = modifiersProp.GetArrayElementAtIndex((int)PuppetStats.HealthPoints);
            prop.floatValue = Mathf.Clamp(EditorGUILayout.IntSlider(Mathf.RoundToInt(prop.floatValue * 100f), -100, 100) * 0.01f, -1f, 1f);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[4], CustomStyles.italicLabel);
            prop = modifiersProp.GetArrayElementAtIndex((int)PuppetStats.Accuracy);
            prop.floatValue = Mathf.Clamp(EditorGUILayout.IntSlider(Mathf.RoundToInt(prop.floatValue * 100f), -100, 100) * 0.01f, -1f, 1f);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[5], CustomStyles.italicLabel);
            prop = modifiersProp.GetArrayElementAtIndex((int)PuppetStats.Initiative);
            prop.floatValue = Mathf.Clamp(EditorGUILayout.IntSlider(Mathf.RoundToInt(prop.floatValue * 100f), -100, 100) * 0.01f, -1f, 1f);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[6], CustomStyles.italicLabel);
            prop = modifiersProp.GetArrayElementAtIndex((int)PuppetStats.Stamina);
            prop.floatValue = Mathf.Clamp(EditorGUILayout.IntSlider(Mathf.RoundToInt(prop.floatValue * 100f), -100, 100) * 0.01f, -1f, 1f);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[7], CustomStyles.italicLabel);
            var minProp = modifiersProp.GetArrayElementAtIndex((int)PuppetStats.MinDamage);
            var maxProp = modifiersProp.GetArrayElementAtIndex((int)PuppetStats.MaxDamage);
            minProp.floatValue = Mathf.Clamp(EditorGUILayout.IntSlider(Mathf.RoundToInt(minProp.floatValue * 100f), -100, 100) * 0.01f, -1f, 1f);
            maxProp.floatValue = minProp.floatValue;

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_self);
            }
        }

        private void OnEnable()
        {
            _self = target as PuppetDisease;
        }
        #endregion
    }

}
