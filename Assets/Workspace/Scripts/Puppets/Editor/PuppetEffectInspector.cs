// <copyright file="PuppetEffectInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;

namespace V4F.Puppets
{

    [CustomEditor(typeof(PuppetEffect)), InitializeOnLoad]
    public class PuppetEffectInspector : Editor
    {
        #region Fields
        private static readonly GUIContent[] __content = null;                

        private PuppetEffect _self = null;
        #endregion

        #region Constructors
        static PuppetEffectInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent("Name:"),
                new GUIContent("Timer:"),
                new GUIContent("Apply damage"),
                new GUIContent("Invert"),
                new GUIContent("Damage:"),
                new GUIContent("min"),
                new GUIContent("max"),
                new GUIContent("Stun"),
                new GUIContent("Resistance:"),
            };
        }
        #endregion

        #region Methods
        //[MenuItem("Assets/Create/V4F/Personage/Effect", false, 802)]
        private static void CreateAsset()
        {
            var asset = ScriptableHelper.CreateAsset<PuppetEffect>();
            Debug.LogFormat("Create \"{0}\" object (ID: {1})", typeof(PuppetEffect).Name, asset.uniqueID);
        }

        public override void OnInspectorGUI()
        {
            var op1 = GUILayout.Width(16f);
            var op2 = GUILayout.Width(32f);

            serializedObject.Update();

            EditorGUILayout.HelpBox(_self.uniqueID, MessageType.None);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

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
            EditorGUILayout.LabelField(__content[8], CustomStyles.italicLabel);
            var resistProp = serializedObject.FindProperty("_resist");
            resistProp.enumValueIndex = (int)((Resists)EditorGUILayout.EnumPopup((Resists)resistProp.enumValueIndex));

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[1], CustomStyles.italicLabel);
            var timerProp = serializedObject.FindProperty("_timer");
            timerProp.intValue = EditorGUILayout.IntSlider(timerProp.intValue, 0, 10);

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            var applyProp = serializedObject.FindProperty("_applyDamage");
            applyProp.boolValue = EditorGUILayout.ToggleLeft(GUIContent.none, applyProp.boolValue, op1);
            EditorGUILayout.LabelField(__content[2], CustomStyles.italicLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!applyProp.boolValue);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            var invertProp = serializedObject.FindProperty("_invertDamage");
            invertProp.boolValue = EditorGUILayout.ToggleLeft(GUIContent.none, invertProp.boolValue, op1);
            EditorGUILayout.LabelField(__content[3], CustomStyles.italicLabel);
            EditorGUILayout.EndHorizontal();                       
                        
            EditorGUILayout.LabelField(__content[4], CustomStyles.italicLabel);
            var minDamageProp = serializedObject.FindProperty("_minDamage");
            var maxDamageProp = serializedObject.FindProperty("_maxDamage");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(__content[5], op2);            
            minDamageProp.intValue = EditorGUILayout.IntSlider(minDamageProp.intValue, 1, maxDamageProp.intValue);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(__content[6], op2);
            maxDamageProp.intValue = EditorGUILayout.IntSlider(maxDamageProp.intValue, minDamageProp.intValue, 100);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            var stunProp = serializedObject.FindProperty("_stun");
            stunProp.boolValue = EditorGUILayout.ToggleLeft(GUIContent.none, stunProp.boolValue, op1);
            EditorGUILayout.LabelField(__content[7], CustomStyles.italicLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_self);
            }
        }

        private void OnEnable()
        {
            _self = target as PuppetEffect;
        }

        private void OnDisable()
        {
            _self = null;
        }
        #endregion
    }

}
