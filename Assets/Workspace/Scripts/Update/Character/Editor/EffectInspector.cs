// <copyright file="EffectInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEditor;

namespace V4F.Character
{

    [CustomEditor(typeof(Effect)), InitializeOnLoad]
    public class EffectInspector : Editor
    {
        #region Fields
        private static readonly GUIContent[] __content = null;
        private Effect _self = null;
        #endregion

        #region Constructors
        static EffectInspector()
        {
            __content = new GUIContent[]
            {
                GUIContent.none,
                new GUIContent("Title:"),
                new GUIContent("Description:"),
                new GUIContent("Resistance:"),
                new GUIContent("Timer:"),                
                new GUIContent("Make damage"),
                new GUIContent("Reverse damage"),                
            };
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Character/Effect", false, 802)]
        private static void CreateAsset()
        {
            ScriptableHelper.CreateAsset<Effect>().Initialize();
        }

        public override void OnInspectorGUI()
        {
            var op1 = GUILayout.Width(16f);            

            serializedObject.Update();

            EditorGUILayout.HelpBox(_self.uniqueID, MessageType.None);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                var titleProp = serializedObject.FindProperty("_title");
                titleProp.stringValue = DrawLocalization(__content[1], titleProp.stringValue);

                EditorGUILayout.Separator();

                var descriptionProp = serializedObject.FindProperty("_description");
                descriptionProp.stringValue = DrawLocalization(__content[2], descriptionProp.stringValue);

                EditorGUILayout.Separator();

                EditorGUILayout.LabelField(__content[3], CustomStyles.italicLabel);
                var resistTypeProp = serializedObject.FindProperty("_resistType");
                resistTypeProp.enumValueIndex = (int)((ResistanceType)EditorGUILayout.EnumPopup((ResistanceType)resistTypeProp.enumValueIndex));

                EditorGUILayout.Separator();

                EditorGUILayout.LabelField(__content[4], CustomStyles.italicLabel);
                var timerProp = serializedObject.FindProperty("_timer");
                timerProp.intValue = EditorGUILayout.IntSlider(timerProp.intValue, 0, 10);

                EditorGUILayout.Separator();
                
                var makeDamageProp = serializedObject.FindProperty("_makeDamage");
                EditorGUILayout.BeginHorizontal();
                makeDamageProp.boolValue = EditorGUILayout.ToggleLeft(__content[0], makeDamageProp.boolValue, op1);
                EditorGUILayout.LabelField(__content[5], CustomStyles.italicLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUI.BeginDisabledGroup(!makeDamageProp.boolValue);
                {                    
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.Space();

                        EditorGUILayout.BeginVertical();
                        {
                            var minDamageProp = serializedObject.FindProperty("_minDamage");
                            var minDamage = minDamageProp.intValue;
                            var maxDamageProp = serializedObject.FindProperty("_maxDamage");
                            var maxDamage = maxDamageProp.intValue;

                            EditorGUILayout.LabelField(string.Format("Damage {0} - {1}", minDamage, maxDamage), CustomStyles.italicLabel);

                            var min = minDamage + 0f;
                            var max = maxDamage + 0f;
                            EditorGUILayout.MinMaxSlider(ref min, ref max, 1, 100);
                            minDamageProp.intValue = Mathf.RoundToInt(min);
                            maxDamageProp.intValue = Mathf.RoundToInt(max);

                            var reverseDamageProp = serializedObject.FindProperty("_reverseDamage");
                            EditorGUILayout.BeginHorizontal();
                            reverseDamageProp.boolValue = EditorGUILayout.ToggleLeft(__content[0], reverseDamageProp.boolValue, op1);
                            EditorGUILayout.LabelField(__content[6], CustomStyles.italicLabel);
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                    }                                        
                    EditorGUILayout.EndHorizontal();                 
                }                
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Separator();                
            }            
            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_self);
            }
        }

        private string DrawLocalization(GUIContent content, string key)
        {
            EditorGUILayout.LabelField(content, CustomStyles.italicLabel);

            var selectedIndex = Localization.GetKeyIndex(key);
            var select = EditorGUILayout.Popup(selectedIndex, Localization.keys);
            if (select != selectedIndex)
            {
                return Localization.GetKey(select);
            }

            return key;
        }

        private void OnEnable()
        {
            _self = target as Effect;
        }        
        #endregion
    }

}
