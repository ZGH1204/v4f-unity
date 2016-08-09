// <copyright file="TouchAdapterInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEditor;

namespace V4F
{

    [CustomEditor(typeof(TouchAdapter)), InitializeOnLoad]
    public class TouchAdapterInspector : Editor
    {
        #region Fields
        private static readonly GUIContent[] __content = null;

        private TouchAdapter _self = null;
        #endregion

        #region Constructors
        static TouchAdapterInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent("Count buttons:"),
                new GUIContent("Count fingers:"),
                new GUIContent("Allowable displacement:"),
                new GUIContent("Long touch detect"),
                new GUIContent("Timer:"),
            };
        }
        #endregion

        #region Methods
        public override void OnInspectorGUI()
        {
            var op1 = GUILayout.Width(16f);

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.Space();

                EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
                {
                    EditorGUILayout.LabelField(__content[0], CustomStyles.italicLabel);
                    var countButtonsProp = serializedObject.FindProperty("_countButtons");
                    countButtonsProp.intValue = EditorGUILayout.IntSlider(countButtonsProp.intValue, 1, 2);

                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(__content[1], CustomStyles.italicLabel);
                    var countFingersProp = serializedObject.FindProperty("_countFingers");
                    countFingersProp.intValue = EditorGUILayout.IntSlider(countFingersProp.intValue, 1, 10);

                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(__content[2], CustomStyles.italicLabel);
                    var allowableDisplacementProp = serializedObject.FindProperty("_allowableDisplacement");
                    allowableDisplacementProp.floatValue = EditorGUILayout.Slider(allowableDisplacementProp.floatValue, 0f, 4f);

                    EditorGUILayout.Separator();
                    var longTouchProp = serializedObject.FindProperty("_longTouch");
                    EditorGUILayout.BeginHorizontal();
                    {
                        longTouchProp.boolValue = EditorGUILayout.ToggleLeft(GUIContent.none, longTouchProp.boolValue, op1);
                        EditorGUILayout.LabelField(__content[3], CustomStyles.italicLabel);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.BeginDisabledGroup(!longTouchProp.boolValue);
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.Space();
                                EditorGUILayout.Space();

                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.LabelField(__content[4], CustomStyles.italicLabel);
                                    var longTouchTimerProp = serializedObject.FindProperty("_longTouchTimer");
                                    longTouchTimerProp.floatValue = EditorGUILayout.Slider(longTouchTimerProp.floatValue, 0f, 2f);
                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.EndDisabledGroup();                               

                EditorGUILayout.Space();
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
            _self = target as TouchAdapter;
        }

        private void OnDisable()
        {
            _self = null;
        }
        #endregion
    }

}
