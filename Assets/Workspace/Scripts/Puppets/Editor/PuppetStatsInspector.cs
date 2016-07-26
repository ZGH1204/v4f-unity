// <copyright file="PuppetStatsInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEditor;

namespace V4F.Puppets
{

    [CustomEditor(typeof(PuppetStats)), InitializeOnLoad]
    public class PuppetStatsInspector : Editor
    {/*
        #region Fields
        private static readonly GUIContent __paramHealthPoints = null;
        private static readonly GUIContent __paramAccuracy = null;
        private static readonly GUIContent __paramInitiative = null;
        private static readonly GUIContent __paramStamina = null;
        private static readonly GUIContent __paramHunger = null;
        private static readonly GUIContent __paramRage = null;

        private static readonly Color __colorParamPositive;
        private static readonly Color __colorParamNegative;
        private static readonly Color __colorParamNormal;

        private PuppetStats _self;
        private Puppet _puppet;
        #endregion

        #region Constructors
        static PuppetStatsInspector()
        {
            __paramHealthPoints = new GUIContent("Health Points:");
            __paramAccuracy = new GUIContent("Accuracy:");
            __paramInitiative = new GUIContent("Initiative:");
            __paramStamina = new GUIContent("Stamina:");
            __paramHunger = new GUIContent("Hunger:");
            __paramRage = new GUIContent("Rage:");

            __colorParamPositive = new Color(0f, 0.9f, 0f);
            __colorParamNegative = new Color(0.9f, 0f, 0f);
            __colorParamNormal = new Color(0f, 0f, 0.2f);
        }
        #endregion

        #region Methods
        public override void OnInspectorGUI()
        {
            var style = new GUIStyle(EditorStyles.label);

            var pop = GUILayout.Width(96f);
            var sop = GUILayout.Width(48f);

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(__paramHealthPoints, CustomStyles.italicLabel, pop);
            GUILayout.FlexibleSpace();
            style.normal.textColor = GetParamColor(_self.healthPointsTotal, _self.healthPointsOrigin);
            EditorGUILayout.LabelField(_self.healthPointsTotal.ToString(), style, sop);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(__paramAccuracy, CustomStyles.italicLabel, pop);
            GUILayout.FlexibleSpace();
            style.normal.textColor = GetParamColor(_self.accuracy, _self.accuracyOrigin);
            EditorGUILayout.LabelField(_self.accuracy.ToString(), style, sop);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(__paramInitiative, CustomStyles.italicLabel, pop);
            GUILayout.FlexibleSpace();
            style.normal.textColor = GetParamColor(_self.initiative, _self.initiativeOrigin);
            EditorGUILayout.LabelField(_self.initiative.ToString(), style, sop);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(__paramStamina, CustomStyles.italicLabel, pop);
            GUILayout.FlexibleSpace();
            style.normal.textColor = GetParamColor(_self.staminaTotal, _self.staminaOrigin);
            EditorGUILayout.LabelField(_self.staminaTotal.ToString(), style, sop);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(__paramHunger, CustomStyles.italicLabel, pop);
            GUILayout.FlexibleSpace();
            style.normal.textColor = GetParamColor(_self.hunger, _self.hunger);
            EditorGUILayout.LabelField(_self.hunger.ToString(), style, sop);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(__paramRage, CustomStyles.italicLabel, pop);
            GUILayout.FlexibleSpace();
            style.normal.textColor = GetParamColor(_self.rage, _self.rage);
            EditorGUILayout.LabelField(_self.rage.ToString(), style, sop);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private Color GetParamColor(int total, int origin)
        {
            if (total > origin)
            {
                return __colorParamPositive;
            }

            if (total < origin)
            {
                return __colorParamNegative;
            }

            return __colorParamNormal;
        }

        private void OnEnable()
        {
            _self = target as PuppetStats;

            serializedObject.Update();

            var puppet = serializedObject.FindProperty("_puppet");
            puppet.objectReferenceValue = _self.GetComponent<Puppet>();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnDisable()
        {

        }
        #endregion
    */}

}
