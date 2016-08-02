// <copyright file="PuppetStorageInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace V4F.Puppets
{

    [CustomEditor(typeof(PuppetStorage)), InitializeOnLoad]
    public class PuppetStorageInspector : Editor
    {
        #region Fields
        private static readonly GUIContent[] __content = null;
        private static readonly float __heightNormal;
        private static readonly Color __colourActive;
        private static readonly Color __colourNormal;

        private static GUIStyle __titleActiveStyle = null;

        private PuppetStorage _self;
        private ReorderableList _puppets;
        private int _lastActiveIndex;
        private List<int> _queueOnRemove;
        private Puppet _puppetForEdit;
        private bool _drawDropArea;
        #endregion

        #region Properties
        private static GUIStyle titleActiveStyle
        {
            get
            {
                if (__titleActiveStyle == null)
                {
                    __titleActiveStyle = new GUIStyle(EditorStyles.boldLabel);
                    __titleActiveStyle.normal.textColor = Color.white;
                }
                return __titleActiveStyle;
            }
        }
        #endregion

        #region Constructors
        static PuppetStorageInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/common_button_edit.png")),
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/common_button_close.png")),
                new GUIContent("Add puppet to storage..."),
            };

            __heightNormal = EditorGUIUtility.singleLineHeight * 4f;
            __colourActive = new Color(0.33f, 0.66f, 1f, 0.82f);
            __colourNormal = new Color(0.43f, 0.43f, 0.43f, 1f);
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Personage/Storage", false, 807)]
        private static void CreateAsset()
        {
            ScriptableHelper.CreateAsset<PuppetStorage>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();            

            EditorGUILayout.Space();
            _puppets.DoLayoutList();

            while (_queueOnRemove.Count > 0)
            {
                var removeIndex = _queueOnRemove[0];
                var index = _puppets.index;

                _puppets.serializedProperty.DeleteArrayElementAtIndex(removeIndex);
                _puppets.serializedProperty.DeleteArrayElementAtIndex(removeIndex);

                serializedObject.ApplyModifiedProperties();

                var upper = _puppets.serializedProperty.arraySize;
                _puppets.index = Mathf.Min(index, (upper - 1));

                _queueOnRemove.RemoveAt(0);
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

            if (_puppetForEdit != null)
            {
                Selection.activeObject = _puppetForEdit;
                EditorGUIUtility.PingObject(_puppetForEdit);
                _puppetForEdit = null;
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
                if ((_self.countPuppets < PuppetSkillSet.MaxSkills) && dropArea.Contains(currentEvent.mousePosition))
                {
                    _drawDropArea = true;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (eventType == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            var puppet = draggedObject as Puppet;
                            if (puppet != null)
                            {
                                if (!_self.Contains(puppet.uniqueID))
                                {
                                    var index = _puppets.serializedProperty.arraySize;
                                    _puppets.serializedProperty.InsertArrayElementAtIndex(index);
                                    _puppets.index = index;

                                    var prop = _puppets.serializedProperty.GetArrayElementAtIndex(index);
                                    prop.objectReferenceValue = puppet;

                                    serializedObject.ApplyModifiedProperties();

                                    _lastActiveIndex = _puppets.index;
                                }
                                else
                                {
                                    Debug.LogWarningFormat("Puppet {0}\"{1}\"{2} already exists in storage!", '{', puppet.uniqueID, '}');
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

            var property = _puppets.serializedProperty.GetArrayElementAtIndex(index);
            var puppet = property.objectReferenceValue as Puppet;
            if (puppet != null)
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

                Sprite icon = puppet.icon;
                if (icon != null)
                {
                    EditorGUI.DrawTextureTransparent(rcIcon, icon.texture, ScaleMode.StretchToFill);
                }
                else
                {
                    EditorGUI.DrawRect(rcIcon, Color.black);
                }

                GUIStyle titleStyle = (isActive ? titleActiveStyle : EditorStyles.label);
                EditorGUI.LabelField(rcTitle, puppet.properName, titleStyle);

                if (GUI.Button(rcEdit, __content[0]))
                {
                    _puppetForEdit = puppet;
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

        private ReorderableList MakePuppetsList()
        {
            var elements = serializedObject.FindProperty("_puppets");
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
                    var puppet = property.objectReferenceValue as Puppet;
                    EditorGUIUtility.PingObject(puppet);
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
            _self = target as PuppetStorage;
            _puppets = MakePuppetsList();
            _lastActiveIndex = _puppets.index;
            _queueOnRemove = new List<int>(8);
            _puppetForEdit = null;
            _drawDropArea = false;
        }
        #endregion
    }

}
