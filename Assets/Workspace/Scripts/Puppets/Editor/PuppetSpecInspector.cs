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
        private static readonly GUIContent __labelСharacteristics = null;
        private static readonly GUIContent __labelHealthPoints = null;
        private static readonly GUIContent __labelAccuracy = null;
        private static readonly GUIContent __labelInitiative = null;

        private PuppetSpec _self;
        #endregion

        #region Constructors
        static PuppetSpecInspector()
        {
            __labelСharacteristics = new GUIContent("Сharacteristics");
            __labelHealthPoints = new GUIContent("Health Points:");
            __labelAccuracy = new GUIContent("Accuracy:");
            __labelInitiative = new GUIContent("Initiative:");
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Puppets/Specification", false, 800)]
        private static void CreateAsset()
        {
            ScriptableHelper.CreateAsset<PuppetSpec>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField(__labelСharacteristics, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField(__labelHealthPoints);
            _self.healthPoints = EditorGUILayout.IntSlider(_self.healthPoints, 1, 100);

            EditorGUILayout.LabelField(__labelAccuracy);
            _self.accuracy = EditorGUILayout.IntSlider(_self.accuracy, 1, 100);

            EditorGUILayout.LabelField(__labelInitiative);
            _self.initiative = EditorGUILayout.IntSlider(_self.initiative, 1, 100);

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
