// <copyright file="PuppetSpecInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEditor;

namespace V4F.Puppets
{
    
    [CustomEditor(typeof(PuppetSpec)), InitializeOnLoad]
    public class PuppetSpecInspector : Editor
    {
        #region Fields
        private static readonly GUIContent[] __content = null;        

        private PuppetSpec _self;
        #endregion

        #region Constructors
        static PuppetSpecInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent("Health Points:"),
                new GUIContent("Accuracy:"),
                new GUIContent("Initiative:"),
                new GUIContent("Stamina:"),
                new GUIContent("Damage:"),
                new GUIContent("min"),
                new GUIContent("max"),
                new GUIContent("Critical (%):"),
                new GUIContent("chance"),
                new GUIContent("damage"),
            };            
        }
        #endregion

        #region Methods
        //[MenuItem("Assets/Create/V4F/Personage/Specification", false, 800)]
        private static void CreateAsset()
        {
            var asset = ScriptableHelper.CreateAsset<PuppetSpec>();
            asset.Initialize();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(__content[0], CustomStyles.italicLabel);
                    _self.SetStat(Stats.HealthPoints, EditorGUILayout.IntSlider(_self.GetStat(Stats.HealthPoints), 1, 100));

                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(__content[1], CustomStyles.italicLabel);
                    _self.SetStat(Stats.Accuracy, EditorGUILayout.IntSlider(_self.GetStat(Stats.Accuracy), 1, 100));

                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(__content[2], CustomStyles.italicLabel);
                    _self.SetStat(Stats.Initiative, EditorGUILayout.IntSlider(_self.GetStat(Stats.Initiative), 1, 100));

                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(__content[3], CustomStyles.italicLabel);
                    _self.SetStat(Stats.Stamina, EditorGUILayout.IntSlider(_self.GetStat(Stats.Stamina), 1, 100));

                    var op1 = GUILayout.Width(32f);
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(__content[4], CustomStyles.italicLabel);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(__content[5], op1);
                    _self.SetStat(Stats.MinDamage, EditorGUILayout.IntSlider(_self.GetStat(Stats.MinDamage), 1, _self.GetStat(Stats.MaxDamage)));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(__content[6], op1);
                    _self.SetStat(Stats.MaxDamage, EditorGUILayout.IntSlider(_self.GetStat(Stats.MaxDamage), _self.GetStat(Stats.MinDamage), 100));
                    EditorGUILayout.EndHorizontal();

                    var op2 = GUILayout.Width(52f);
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(__content[7], CustomStyles.italicLabel);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(__content[8], op2);
                    _self.SetStat(Stats.CriticalChance, EditorGUILayout.IntSlider(_self.GetStat(Stats.CriticalChance), 0, 200));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(__content[9], op2);
                    _self.SetStat(Stats.CriticalDamage, EditorGUILayout.IntSlider(_self.GetStat(Stats.CriticalDamage), 0, 200));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();
            }           
            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_self);                
            }
        }

        private void OnEnable()
        {
            _self = target as PuppetSpec;
        }        
        #endregion
    }

}
