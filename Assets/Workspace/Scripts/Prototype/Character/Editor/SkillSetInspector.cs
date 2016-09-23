// <copyright file="SkillSetInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace V4F.Character
{

    [CustomEditor(typeof(SkillSet)), InitializeOnLoad]
    public class SkillSetInspector : Editor
    {
        #region Fields
        private static readonly GUIContent[] __content = null;
        private static readonly float __heightNormal;
        private static readonly Color __colourActive;
        private static readonly Color __colourNormal;
        private static GUIStyle __titleActiveSkillStyle = null;
        private SkillSet _self;
        private ReorderableList _skills;
        private Skill _skillForEdit;
        private List<int> _queueOnRemove;
        private int _lastActiveIndex;
        private bool _drawDropArea;
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
        static SkillSetInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/common_button_edit.png")),
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/common_button_close.png")),
                new GUIContent("Add skill to set..."),
            };

            __heightNormal = EditorGUIUtility.singleLineHeight * 4f;
            __colourActive = new Color(0.33f, 0.66f, 1f, 0.82f);
            __colourNormal = new Color(0.43f, 0.43f, 0.43f, 1f);
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Character/SkillSet", false, 804)]
        private static void CreateAsset()
        {
            ScriptableHelper.CreateAsset<SkillSet>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            string message = string.Format("{0} skill(s) left.", (SkillSet.MaxSkills - _self.countSkills));
            EditorGUILayout.HelpBox(message, MessageType.Info);

            EditorGUILayout.Space();
            _skills.DoLayoutList();
            while (_queueOnRemove.Count > 0)
            {
                var removeIndex = _queueOnRemove[0];
                var index = _skills.index;

                _skills.serializedProperty.DeleteArrayElementAtIndex(removeIndex);
                _skills.serializedProperty.DeleteArrayElementAtIndex(removeIndex);

                var upper = _skills.serializedProperty.arraySize;
                _skills.index = Mathf.Min(index, (upper - 1));

                _queueOnRemove.RemoveAt(0);

                serializedObject.ApplyModifiedProperties();
            }

            var dropArea = GUILayoutUtility.GetRect(0f, 56f, GUILayout.ExpandWidth(true));
            if (EventsHandler(dropArea))
            {
                EditorGUI.DrawRect(dropArea, new Color(0f, 0.75f, 0f, 0.25f));
                EditorGUI.LabelField(dropArea, __content[2], CustomStyles.infoDrop);
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_self);
            }

            if (_skillForEdit != null)
            {
                Selection.activeObject = _skillForEdit;
                EditorGUIUtility.PingObject(_skillForEdit);
                _skillForEdit = null;
            }
        }

        private ReorderableList MakeSkillsList()
        {
            var elements = serializedObject.FindProperty("_skills");
            var reorderList = new ReorderableList(serializedObject, elements, true, false, false, false);

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
                    var skill = property.objectReferenceValue as Skill;
                    EditorGUIUtility.PingObject(skill);
                }
                else
                {
                    _lastActiveIndex = index;
                }
            };

            return reorderList;
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

            var property = _skills.serializedProperty.GetArrayElementAtIndex(index);
            var skill = property.objectReferenceValue as Skill;
            if (skill != null)
            {
                rect.height = __heightNormal;

                var rcIcon = new Rect(rect.x + 2f, rect.y + 4f, rect.height - 8f, rect.height - 8f);
                var rcTitle = new Rect(
                    rcIcon.x + rcIcon.width + 8f,
                    rect.y + (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f,
                    rect.width - (rcIcon.x + rcIcon.width + 24f),
                    EditorGUIUtility.singleLineHeight * (isActive ? 1.25f : 1f));
                var rcEdit = new Rect(rcTitle.x + rcTitle.width + 2f, rect.y + 4f, 24f, 24f);
                var rcCross = new Rect(rcEdit.x + rcEdit.width + 2f, rect.y + 4f, 24f, 24f);

                var icon = skill[0].icon;
                if (icon != null)
                {
                    var iW = 1f / icon.texture.width;
                    var iH = 1f / icon.texture.height;
                    var texCoords = new Rect();
                    texCoords.xMin = icon.textureRect.xMin * iW;
                    texCoords.xMax = icon.textureRect.xMax * iW;
                    texCoords.yMin = icon.textureRect.yMin * iH;
                    texCoords.yMax = icon.textureRect.yMax * iH;
                    GUI.DrawTextureWithTexCoords(rcIcon, icon.texture, texCoords);
                }
                else
                {
                    EditorGUI.DrawRect(rcIcon, Color.black);
                }

                GUIStyle titleStyle = (isActive ? titleActiveSkillStyle : EditorStyles.label);
                EditorGUI.LabelField(rcTitle, skill.name, titleStyle);

                if (GUI.Button(rcEdit, __content[0]))
                {
                    _skillForEdit = skill;
                }

                if (GUI.Button(rcCross, __content[1]))
                {
                    _queueOnRemove.Insert(0, index);
                }
            }
            else
            {
                _queueOnRemove.Insert(0, index);
            }
        }

        private bool EventsHandler(Rect dropArea)
        {
            var currentEvent = Event.current;
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            var eventType = currentEvent.GetTypeForControl(controlID);

            if (eventType == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(controlID);
            }

            if ((eventType == EventType.DragPerform) || (eventType == EventType.DragUpdated))
            {
                if ((_self.countSkills < SkillSet.MaxSkills) && dropArea.Contains(currentEvent.mousePosition))
                {
                    _drawDropArea = true;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (eventType == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            Skill skill = draggedObject as Skill;
                            if (skill != null)
                            {
                                if (skill.editCompleted)
                                {
                                    var index = _skills.serializedProperty.arraySize;
                                    _skills.serializedProperty.InsertArrayElementAtIndex(index);
                                    _skills.index = index;

                                    var prop = _skills.serializedProperty.GetArrayElementAtIndex(index);
                                    prop.objectReferenceValue = skill;

                                    _lastActiveIndex = _skills.index;

                                    serializedObject.ApplyModifiedProperties();
                                }
                                else
                                {
                                    Debug.LogErrorFormat("Add skill to skillset: skill \"{0}\" is not prepared!", skill.name);
                                }                                
                            }
                        }

                        _drawDropArea = false;
                    }
                }
                else
                {
                    _drawDropArea = false;
                }
            }

            if (eventType == EventType.DragExited)
            {
                _drawDropArea = false;
            }

            return _drawDropArea;
        }

        private void OnEnable()
        {
            _self = target as SkillSet;
            _skills = MakeSkillsList();
            _skillForEdit = null;
            _queueOnRemove = new List<int>(SkillSet.MaxSkills * 2);
            _lastActiveIndex = _skills.index;
            _drawDropArea = false;
        }
        #endregion
    }

}
