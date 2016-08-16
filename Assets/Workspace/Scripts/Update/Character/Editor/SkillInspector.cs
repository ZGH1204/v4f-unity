// <copyright file="SkillInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEditor;

namespace V4F.Character
{

    [CustomEditor(typeof(Skill)), InitializeOnLoad]
    public class SkillInspector : Editor
    {
        #region Fields
        private static readonly GUIContent[] __content = null;
        private static readonly GUIContent[] __stages = null;
        private Skill _self = null;
        private int _stage = -1;
        #endregion

        #region Constructors
        static SkillInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent("Stage:"),
                GUIContent.none,                
            };

            __stages = new GUIContent[Skill.AllStages];
            for (var i = 0; i < Skill.AllStages; ++i)
            {
                __stages[i] = new GUIContent(string.Format("Level {0}", (i + 1)));
            }
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Character/Skill", false, 801)]
        private static void CreateAsset()
        {
            ScriptableHelper.CreateAsset<Skill>().Initialize();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox(_self.uniqueID, MessageType.None);

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField(__content[0], CustomStyles.italicLabel);
                _stage = EditorGUILayout.Popup(_stage, __stages);
                if (_stage != -1)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {

                    }
                    EditorGUILayout.EndVertical();
                }
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
            _self = target as Skill;
        }
        #endregion
    }

}
