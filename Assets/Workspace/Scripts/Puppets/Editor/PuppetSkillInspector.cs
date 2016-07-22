// <copyright file="PuppetSkillInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace V4F.Puppets
{

    [CustomEditor(typeof(PuppetSkill)), InitializeOnLoad]
    public class PuppetSkillInspector : Editor
    {        
        #region Fields
        private static readonly GUIContent  __labelUIX = null;
        private static readonly GUIContent  __labelTitle = null;
        private static readonly GUIContent  __paramIcon = null;
        private static readonly GUIContent  __labelProperties = null;
        private static readonly GUIContent  __paramUseOnTarget = null;
        private static readonly GUIContent  __labelTargetsQuantity = null;
        private static readonly GUIContent  __labelСanUseInPositions = null;
        private static readonly GUIContent  __labelСanUseToTargetPositions = null;
        private static readonly GUIContent  __paramPosition1 = null;
        private static readonly GUIContent  __paramPosition2 = null;
        private static readonly GUIContent  __paramPosition3 = null;
        private static readonly GUIContent  __paramPosition4 = null;
        private static readonly GUIContent  __labelDamageModifier = null;
        private static readonly GUIContent  __labelAccuracyModifier = null;
        private static readonly GUIContent  __labelCritChanceModifier = null;
        private static readonly GUIContent  __labelCritDamageModifier = null;
        private static readonly GUIContent  __labelEffects = null;
        private static readonly GUIContent  __buttonEditing = null;        
        private static readonly GUIContent  __buttonDelEffect = null;
        private static readonly GUIContent  __labelInfoDrop = null;
        private static readonly GUIContent  __togglePassedOn = null;
        private static readonly GUIContent  __togglePassedOff = null;
        private static readonly float       __heightActive;
        private static readonly float       __heightNormal;
        private static readonly Color       __colourActive;
        private static readonly Color       __colourNormal;

        private static GUIStyle __togglePassedStyle = null;

        private PuppetSkill _self;
        private bool[] _foldouts;
        private ReorderableList _effects;
        private int _lastActiveIndex;
        private bool _drawDropArea;
        #endregion

        #region Properties
        private GUIStyle togglePassedStyle
        {
            get
            {
                if (__togglePassedStyle == null)
                {
                    var tex_on = __togglePassedOn.image as Texture2D;
                    var tex_off = __togglePassedOff.image as Texture2D;

                    __togglePassedStyle = new GUIStyle(GUI.skin.button);
                    __togglePassedStyle.normal.background = tex_off;
                    __togglePassedStyle.hover.background = tex_off;
                    __togglePassedStyle.focused.background = tex_off;
                    __togglePassedStyle.active.background = tex_on;
                    __togglePassedStyle.onNormal.background = tex_on;
                    __togglePassedStyle.onHover.background = tex_on;
                    __togglePassedStyle.onFocused.background = tex_on;
                    __togglePassedStyle.onActive.background = tex_on;                    
                }
                return __togglePassedStyle;
            }
        }
        #endregion

        #region Constructors
        static PuppetSkillInspector()
        {
            __labelUIX = new GUIContent("UIX");
            __labelTitle = new GUIContent("Title:");
            __paramIcon = new GUIContent(GUIContent.none);
            __labelProperties = new GUIContent("Properties");
            __paramUseOnTarget = new GUIContent("Use on:");
            __labelTargetsQuantity = new GUIContent("Targets quantity:");
            __labelСanUseInPositions = new GUIContent("Сan use in positions:");
            __labelСanUseToTargetPositions = new GUIContent("Сan use to target positions:");
            __paramPosition1 = new GUIContent("1");
            __paramPosition2 = new GUIContent("2");
            __paramPosition3 = new GUIContent("3");
            __paramPosition4 = new GUIContent("4");
            __labelDamageModifier = new GUIContent("Damage modifier (%):");
            __labelAccuracyModifier = new GUIContent("Accuracy modifier (%):");
            __labelCritChanceModifier = new GUIContent("Critical chance modifier (%):");
            __labelCritDamageModifier = new GUIContent("Critical damage modifier:");
            __labelEffects = new GUIContent("Effects");
            __buttonEditing = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/toolbar_button_edit.png"));            
            __buttonDelEffect = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/toolbar_button_trash.png"));
            __labelInfoDrop = new GUIContent("Add effect to list...");
            __togglePassedOn = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/item_passed_on.png"));
            __togglePassedOff = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/item_passed_off.png"));
            __heightNormal = EditorGUIUtility.singleLineHeight * 1.5f;
            __colourActive = new Color(0.33f, 0.66f, 1f, 0.82f);
            __colourNormal = new Color(0.43f, 0.43f, 0.43f, 1f);
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Puppets/Skill", false, 800)]
        private static void CreateAsset()
        {
            ScriptableHelper.CreateAsset<PuppetSkill>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();

            // --------------------------------------------------------------
            _foldouts[0] = EditorGUILayout.Foldout(_foldouts[0], __labelUIX, CustomStyles.boldFoldout);
            if (_foldouts[0])
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                UIXHandler();
                EditorGUILayout.EndVertical();
            }            

            // --------------------------------------------------------------
            EditorGUILayout.Space();
            _foldouts[1] = EditorGUILayout.Foldout(_foldouts[1], __labelProperties, CustomStyles.boldFoldout);
            if (_foldouts[1])
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                PropertiesHandler();
                EditorGUILayout.EndVertical();
            }                        

            // --------------------------------------------------------------
            EditorGUILayout.Space();
            _foldouts[2] = EditorGUILayout.Foldout(_foldouts[2], __labelEffects, CustomStyles.boldFoldout);
            if (_foldouts[2])
            {
                EditorGUILayout.BeginVertical(/*EditorStyles.helpBox*/);
                EffectsListHandler();
                EditorGUILayout.EndVertical();
            }            

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_self);
            }
        }

        private ReorderableList MakeEffectsList()
        {
            var elements = serializedObject.FindProperty("_effects");
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
                    var effect = property.objectReferenceValue as PuppetEffect;
                    EditorGUIUtility.PingObject(effect);
                }
                else
                {
                    _lastActiveIndex = index;
                }
            };

            reorderList.onReorderCallback = (ReorderableList list) =>
            {
                var index = list.index;
                var property = serializedObject.FindProperty("_effectsPassed");

                var item = property.GetArrayElementAtIndex(_lastActiveIndex);
                var temp = item.boolValue;

                property.DeleteArrayElementAtIndex(_lastActiveIndex);
                property.InsertArrayElementAtIndex(index);

                item = property.GetArrayElementAtIndex(index);
                item.boolValue = temp;

                serializedObject.ApplyModifiedProperties();
            };

            return reorderList;
        }

        private float ElementHeight(int index)
        {
            return __heightNormal;
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

            var property = _effects.serializedProperty.GetArrayElementAtIndex(index);
            var effect = property.objectReferenceValue as PuppetEffect;

            property = serializedObject.FindProperty("_effectsPassed");
            property = property.GetArrayElementAtIndex(index);

            var size = Mathf.Min(16f, EditorGUIUtility.singleLineHeight);
            var offset = 2f;
            rect.y += 3f;
            
            var rcTitle = new Rect(rect.x + offset, rect.y, rect.width - 32f, rect.height);
            offset += rcTitle.width + 2f;
            var rcPassed = new Rect(rect.x + offset, rect.y, size, size);
            offset += rcPassed.width + 2f;

            GUIStyle titleStyle = new GUIStyle(EditorStyles.miniBoldLabel);
            if (isActive)
            {
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.normal.textColor = Color.white;
            }
            else if (!property.boolValue)
            {
                titleStyle.normal.textColor = Color.gray;
            }

            EditorGUI.LabelField(rcTitle, effect.title, titleStyle);

            EditorGUI.BeginChangeCheck();
            property.boolValue = EditorGUI.Toggle(rcPassed, GUIContent.none, property.boolValue, togglePassedStyle);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void EditEffectHandler()
        {
            var index = _effects.index;
            var property = _effects.serializedProperty.GetArrayElementAtIndex(index);
            var effect = property.objectReferenceValue as PuppetEffect;

            Selection.activeObject = effect;
            EditorGUIUtility.PingObject(effect);
        }

        private void RemoveEffectHandler()
        {            
            var index = _effects.index;

            _effects.serializedProperty.DeleteArrayElementAtIndex(index);
            _effects.serializedProperty.DeleteArrayElementAtIndex(index);            

            var prop = serializedObject.FindProperty("_effectsPassed");
            prop.DeleteArrayElementAtIndex(index);            

            serializedObject.ApplyModifiedProperties();

            var upper = _effects.serializedProperty.arraySize;
            _effects.index = Mathf.Min(index, (upper - 1));
        }

        private void UIXHandler()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelTitle, CustomStyles.italicLabel);
            var title = Regex.Replace(EditorGUILayout.DelayedTextField(_self.title), @"[^a-zA-Z0-9_ ]", "").Trim();
            if (!string.IsNullOrEmpty(title) && (title.Length > 3))
            {
                _self.title = title;
            }

            EditorGUILayout.Space();
            _self.icon = EditorGUILayout.ObjectField(__paramIcon, _self.icon, typeof(Sprite), false) as Sprite;

            EditorGUILayout.Space();
        }

        private void PropertiesHandler()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__paramUseOnTarget, CustomStyles.italicLabel);
            _self.useOnTarget = (PuppetSkillTarget)EditorGUILayout.EnumPopup(_self.useOnTarget);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelTargetsQuantity, CustomStyles.italicLabel);
            _self.targetsQuantity = EditorGUILayout.IntSlider(_self.targetsQuantity, 1, 4);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelСanUseInPositions, CustomStyles.italicLabel);
            EditorGUILayout.BeginHorizontal();
            _self.UseInPosition(1, EditorGUILayout.ToggleLeft(__paramPosition1, _self.CanUseInPosition(1), GUILayout.Width(32f)));
            _self.UseInPosition(2, EditorGUILayout.ToggleLeft(__paramPosition2, _self.CanUseInPosition(2), GUILayout.Width(32f)));
            _self.UseInPosition(3, EditorGUILayout.ToggleLeft(__paramPosition3, _self.CanUseInPosition(3), GUILayout.Width(32f)));
            _self.UseInPosition(4, EditorGUILayout.ToggleLeft(__paramPosition4, _self.CanUseInPosition(4), GUILayout.Width(32f)));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelСanUseToTargetPositions, CustomStyles.italicLabel);
            EditorGUILayout.BeginHorizontal();
            _self.UseToTargetPosition(1, EditorGUILayout.ToggleLeft(__paramPosition1, _self.CanUseToTargetPosition(1), GUILayout.Width(32f)));
            _self.UseToTargetPosition(2, EditorGUILayout.ToggleLeft(__paramPosition2, _self.CanUseToTargetPosition(2), GUILayout.Width(32f)));
            _self.UseToTargetPosition(3, EditorGUILayout.ToggleLeft(__paramPosition3, _self.CanUseToTargetPosition(3), GUILayout.Width(32f)));
            _self.UseToTargetPosition(4, EditorGUILayout.ToggleLeft(__paramPosition4, _self.CanUseToTargetPosition(4), GUILayout.Width(32f)));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelDamageModifier, CustomStyles.italicLabel);
            _self.damageModifier = EditorGUILayout.Slider(_self.damageModifier * 100f, 0f, 100f) * 0.01f;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelAccuracyModifier, CustomStyles.italicLabel);
            _self.accuracyModifier = EditorGUILayout.Slider(_self.accuracyModifier * 100f, 0f, 100f) * 0.01f;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelCritChanceModifier, CustomStyles.italicLabel);
            _self.critChanceModifier = EditorGUILayout.Slider(_self.critChanceModifier * 100f, 0f, 100f) * 0.01f;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelCritDamageModifier, CustomStyles.italicLabel);
            _self.critDamageModifier = EditorGUILayout.Slider(_self.critDamageModifier, 0f, 100f);

            EditorGUILayout.Space();
        }

        private void EffectsListHandler()
        {
            var toolbarButtonOp = new GUILayoutOption[]
            {
                GUILayout.MinWidth(42f),
                GUILayout.MaxWidth(42f),
            };

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));

            var count = _self.countEffects;
            var stats = string.Format("count: {0} / {1}", count, PuppetSkill.MaxEffects);
            EditorGUILayout.LabelField(stats, CustomStyles.italicLabel, GUILayout.Width(96f));

            GUILayout.FlexibleSpace();

            var editingDisabled = (_effects.index < 0);
            EditorGUI.BeginDisabledGroup(editingDisabled);
            if (GUILayout.Button(__buttonEditing, EditorStyles.toolbarButton, toolbarButtonOp))
            {
                EditEffectHandler();
            }
            EditorGUI.EndDisabledGroup();

            var removeDisabled = (_effects.index < 0);
            EditorGUI.BeginDisabledGroup(removeDisabled);
            if (GUILayout.Button(__buttonDelEffect, EditorStyles.toolbarButton, toolbarButtonOp))
            {
                RemoveEffectHandler();
            }
            EditorGUI.EndDisabledGroup();            

            EditorGUILayout.EndHorizontal();

            _effects.DoLayoutList();

            var dropArea = GUILayoutUtility.GetRect(0f, 48f, GUILayout.ExpandWidth(true));
            if (EffectsEventHandler(dropArea))
            {
                EditorGUI.DrawRect(dropArea, new Color(0f, 0.75f, 0f, 0.25f));
                EditorGUI.LabelField(dropArea, __labelInfoDrop, CustomStyles.infoDrop);
            }

            EditorGUILayout.Space();
        }

        private bool EffectsEventHandler(Rect dropArea)
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
                if ((_self.countEffects < PuppetSkill.MaxEffects) && dropArea.Contains(currentEvent.mousePosition))
                {
                    _drawDropArea = true;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (eventType == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            PuppetEffect effect = draggedObject as PuppetEffect;
                            if (effect != null)
                            {
                                var index = _effects.serializedProperty.arraySize;
                                _effects.serializedProperty.InsertArrayElementAtIndex(index);
                                _effects.index = index;

                                var prop = _effects.serializedProperty.GetArrayElementAtIndex(index);
                                prop.objectReferenceValue = effect;

                                prop = serializedObject.FindProperty("_effectsPassed");
                                prop.InsertArrayElementAtIndex(index);
                                prop = prop.GetArrayElementAtIndex(index);
                                prop.boolValue = true;

                                serializedObject.ApplyModifiedProperties();

                                _lastActiveIndex = _effects.index;
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
            _self = target as PuppetSkill;
            _foldouts = new bool[] { true, true, true };
            _effects = MakeEffectsList();
            _lastActiveIndex = _effects.index;
            _drawDropArea = false;
        }

        private void OnDisbale()
        {
            _effects = null;
            _foldouts = null;
            _self = null;
        }
        #endregion
    }

}
