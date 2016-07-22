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
        private static readonly GUIContent __labelTitle = null;
        private static readonly GUIContent __labelTimer = null;
        private static readonly GUIContent __labelMinDamage = null;
        private static readonly GUIContent __labelMaxDamage = null;
        private static readonly GUIContent __labelApplyDamage = null;
        private static readonly GUIContent __labelInvertDamage = null;
        private static readonly GUIContent __labelStun = null;

        private static readonly GUILayoutOption __toggleOp = null;

        private PuppetEffect _self = null;
        #endregion

        #region Constructors
        static PuppetEffectInspector()
        {
            __labelTitle = new GUIContent("Name:");
            __labelTimer = new GUIContent("Timer:");
            __labelMinDamage = new GUIContent("Мinimum damage:");
            __labelMaxDamage = new GUIContent("Мaximum damage:");
            __labelApplyDamage = new GUIContent("Apply damage");
            __labelInvertDamage = new GUIContent("Invert");
            __labelStun = new GUIContent("Stun");

            __toggleOp = GUILayout.Width(16f);
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Puppets/Effect", false, 800)]
        private static void CreateAsset()
        {
            ScriptableHelper.CreateAsset<PuppetEffect>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelTitle, CustomStyles.italicLabel);
            var title = Regex.Replace(EditorGUILayout.DelayedTextField(_self.title), @"[^a-zA-Z0-9_ ]", "").Trim();
            if (!string.IsNullOrEmpty(title) && (title.Length > 3))
            {
                _self.title = title;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelTimer, CustomStyles.italicLabel);
            _self.timer = EditorGUILayout.IntSlider(_self.timer, 0, 10);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            _self.applyDamage = EditorGUILayout.ToggleLeft(GUIContent.none, _self.applyDamage, __toggleOp);
            EditorGUILayout.LabelField(__labelApplyDamage, CustomStyles.italicLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!_self.applyDamage);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            _self.invertDamage = EditorGUILayout.ToggleLeft(GUIContent.none, _self.invertDamage, __toggleOp);
            EditorGUILayout.LabelField(__labelInvertDamage, CustomStyles.italicLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField(__labelMinDamage, CustomStyles.italicLabel);
            _self.minDamage = EditorGUILayout.IntSlider(_self.minDamage, 0, _self.maxDamage);

            EditorGUILayout.LabelField(__labelMaxDamage, CustomStyles.italicLabel);
            _self.maxDamage = EditorGUILayout.IntSlider(_self.maxDamage, _self.minDamage, 100);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            _self.stun = EditorGUILayout.ToggleLeft(GUIContent.none, _self.stun, __toggleOp);
            EditorGUILayout.LabelField(__labelStun, CustomStyles.italicLabel);
            EditorGUILayout.EndHorizontal();

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
