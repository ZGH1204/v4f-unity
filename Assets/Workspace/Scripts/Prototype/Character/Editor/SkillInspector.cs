// <copyright file="SkillInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace V4F.Character
{

    [CustomEditor(typeof(Skill)), InitializeOnLoad]
    public class SkillInspector : Editor
    {
        #region Fields
        private static readonly GUIContent[] __content = null;        
        private static readonly float __heightNormal;
        private static readonly Color __colourActive;
        private static readonly Color __colourNormal;
        private static readonly Color __colourCompleted;
        private static readonly Color __colourError;
        private static GUIStyle __titleActiveSkillStyle = null;
        private Skill _self = null;
        private ReorderableList _stages = null;
        private SkillStage _stageForEdit;
        private List<int> _queueOnRemove;
        private int _lastActiveIndex;
        #endregion

        #region Properties
        private static GUIStyle titleActiveSkillStyle
        {
            get
            {
                if (__titleActiveSkillStyle == null)
                {
                    __titleActiveSkillStyle = new GUIStyle(EditorStyles.boldLabel);
                    __titleActiveSkillStyle.normal.textColor = Color.white;
                }
                return __titleActiveSkillStyle;
            }
        }
        #endregion

        #region Constructors
        static SkillInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/common_button_edit.png")),
            };

            __heightNormal = EditorGUIUtility.singleLineHeight * 4f;
            __colourActive = new Color(0.33f, 0.66f, 1f, 0.82f);
            __colourNormal = new Color(0.43f, 0.43f, 0.43f, 1f);

            __colourCompleted = new Color32(46, 204, 113, 255);
            __colourError = new Color32(192, 57, 43, 255);
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Character/Skill", false, 803)]
        private static void CreateAsset()
        {
            var skill = ScriptableHelper.CreateAsset<Skill>();
            var skillObject = new SerializedObject(skill);
            var stagesProp = skillObject.FindProperty("_stages");
            for (var i = 0; i < Skill.AllStages; ++i)
            {                
                var stage = CreateInstance<SkillStage>();
                stage.name = string.Format("Stage{0}", i + 1);
                AssetDatabase.AddObjectToAsset(stage, skill);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(stage));

                stagesProp.InsertArrayElementAtIndex(i);
                var stageProp = stagesProp.GetArrayElementAtIndex(i);
                stageProp.objectReferenceValue = stage;

                var stageObject = new SerializedObject(stage);
                stageObject.FindProperty("_parent").objectReferenceValue = skill;
                stageObject.ApplyModifiedProperties();

                skillObject.ApplyModifiedProperties();
            }            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox(_self.uniqueID, MessageType.None);
            EditorGUILayout.Space();
            
            _stages.DoLayoutList();
            while (_queueOnRemove.Count > 0)
            {
                var removeIndex = _queueOnRemove[0];
                var index = _stages.index;

                _stages.serializedProperty.DeleteArrayElementAtIndex(removeIndex);
                _stages.serializedProperty.DeleteArrayElementAtIndex(removeIndex);

                var upper = _stages.serializedProperty.arraySize;
                _stages.index = Mathf.Min(index, (upper - 1));

                _queueOnRemove.RemoveAt(0);

                serializedObject.ApplyModifiedProperties();
            }

            if (_stageForEdit != null)
            {
                Selection.activeObject = _stageForEdit;
                EditorGUIUtility.PingObject(_stageForEdit);
                _stageForEdit = null;
            }            

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_self);
            }            
        }

        private float ElementHeight(int index)
        {
            return __heightNormal + 4f;
        }

        private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index < 0)
            {
                return;
            }

            rect.height = __heightNormal;

            if (isActive)
            {
                var rcInner = new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f);

                EditorGUI.DrawRect(rect, Color.black);
                EditorGUI.DrawRect(rcInner, __colourActive);
            }
            else
            {
                EditorGUI.DrawRect(rect, __colourNormal);
            }
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index < 0)
            {
                return;
            }

            var property = _stages.serializedProperty.GetArrayElementAtIndex(index);
            var stage = property.objectReferenceValue as SkillStage;
            if (stage != null)
            {
                rect.height = __heightNormal;

                var rcIcon = new Rect(rect.x + 2f, rect.y + 4f, rect.height - 8f, rect.height - 8f);
                var rcTitle = new Rect(
                    rcIcon.x + rcIcon.width + 8f,
                    rect.y + (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f,
                    rect.width - (rcIcon.x + rcIcon.width + 38f),
                    EditorGUIUtility.singleLineHeight * (isActive ? 1.25f : 1f));
                var rcEdit = new Rect(rcTitle.x + rcTitle.width + 2f, rect.y + 4f, 24f, 24f);
                var rcCross = new Rect(rcEdit.x + rcEdit.width + 2f, rect.y + 4f, 24f, 24f);
                var rcInner = new Rect(rcEdit.x + 1f, rcEdit.y +1f, rcEdit.width - 2f, rcEdit.height - 2f);

                Sprite icon = stage.icon;
                if (icon != null)
                {
                    EditorGUI.DrawTextureTransparent(rcIcon, icon.texture, ScaleMode.StretchToFill);
                }
                else
                {
                    EditorGUI.DrawRect(rcIcon, Color.black);
                }

                GUIStyle titleStyle = (isActive ? titleActiveSkillStyle : EditorStyles.label);
                EditorGUI.LabelField(rcTitle, stage.name, titleStyle);

                EditorGUI.DrawRect(rcEdit, Color.black);
                EditorGUI.DrawRect(rcInner, ((stage.validate) ? __colourCompleted : __colourError));

                if (GUI.Button(rcCross, __content[0]))
                {
                    _stageForEdit = stage;
                }                
            }
            else
            {
                _queueOnRemove.Insert(0, index);
            }
        }

        private ReorderableList MakeSkillsList()
        {
            var elements = serializedObject.FindProperty("_stages");
            var reorderList = new ReorderableList(serializedObject, elements, false, false, false, false);

            reorderList.showDefaultBackground = false;
            reorderList.headerHeight = 0f;
            reorderList.footerHeight = 0f;
            reorderList.index = -1;

            reorderList.elementHeightCallback = ElementHeight;
            reorderList.drawElementBackgroundCallback = DrawElementBackground;
            reorderList.drawElementCallback = DrawElement;

            reorderList.onSelectCallback = (ReorderableList list) =>
            {
                var index = list.index;
                if (_lastActiveIndex == index)
                {
                    var property = list.serializedProperty.GetArrayElementAtIndex(list.index);
                    var stage = property.objectReferenceValue as SkillStage;
                    EditorGUIUtility.PingObject(stage);
                }
                else
                {
                    _lastActiveIndex = index;
                }
            };

            return reorderList;
        }

        private void OnEnable()
        {
            _self = target as Skill;
            _stages = MakeSkillsList();
            _stageForEdit = null;
            _queueOnRemove = new List<int>(Skill.AllStages);
            _lastActiveIndex = -1;
        }
        #endregion
    }

}
