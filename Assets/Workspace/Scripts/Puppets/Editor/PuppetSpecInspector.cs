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
                new GUIContent("Сharacteristics"),
                new GUIContent("Health Points:"),
                new GUIContent("Accuracy:"),
                new GUIContent("Initiative:"),
                new GUIContent("Stamina:"),
                new GUIContent("Damage:"),
                new GUIContent("min"),
                new GUIContent("max"),
            };            
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Personage/Specification", false, 800)]
        private static void CreateAsset()
        {
            var asset = ScriptableHelper.CreateAsset<PuppetSpec>();
            asset.Initialize();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__content[0], EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[1], CustomStyles.italicLabel);
            _self.SetStat(PuppetStats.HealthPoints, EditorGUILayout.IntSlider(_self.GetStat(PuppetStats.HealthPoints), 1, 100));

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[2], CustomStyles.italicLabel);
            _self.SetStat(PuppetStats.Accuracy, EditorGUILayout.IntSlider(_self.GetStat(PuppetStats.Accuracy), 1, 100));

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[3], CustomStyles.italicLabel);
            _self.SetStat(PuppetStats.Initiative, EditorGUILayout.IntSlider(_self.GetStat(PuppetStats.Initiative), 1, 100));

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[4], CustomStyles.italicLabel);
            _self.SetStat(PuppetStats.Stamina, EditorGUILayout.IntSlider(_self.GetStat(PuppetStats.Stamina), 1, 100));

            var op1 = GUILayout.Width(32f);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[5], CustomStyles.italicLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(__content[6], op1);
            _self.SetStat(PuppetStats.MinDamage, EditorGUILayout.IntSlider(_self.GetStat(PuppetStats.MinDamage), 1, _self.GetStat(PuppetStats.MaxDamage)));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(__content[7], op1);
            _self.SetStat(PuppetStats.MaxDamage, EditorGUILayout.IntSlider(_self.GetStat(PuppetStats.MaxDamage), _self.GetStat(PuppetStats.MinDamage), 100));
            EditorGUILayout.EndHorizontal();

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
            _self = target as PuppetSpec;
        }        
        #endregion
    }

}
