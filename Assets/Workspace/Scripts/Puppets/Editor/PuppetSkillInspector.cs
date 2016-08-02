// <copyright file="PuppetSkillInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Text.RegularExpressions;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace V4F.Puppets
{

    [CustomEditor(typeof(PuppetSkill)), InitializeOnLoad]
    public class PuppetSkillInspector : Editor
    {
        #region Fields
        private static readonly GUIContent[] __content = null;        
        private static readonly float __heightActive;
        private static readonly float __heightNormal;
        private static readonly Color __colourActive;
        private static readonly Color __colourNormal;

        private static GUIStyle __togglePassedStyle = null;

        private PuppetSkill _self;
        private bool[] _foldouts;
        private ReorderableList _effects;
        private List<int> _queueOnRemove;
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
                    var tex_on = __content[19].image as Texture2D;
                    var tex_off = __content[20].image as Texture2D;

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
            __content = new GUIContent[]
            {
                new GUIContent("UIX"),
                new GUIContent("Properties"),
                new GUIContent("Effects"),
                new GUIContent("Title:"),
                new GUIContent(GUIContent.none),
                new GUIContent("Use to:"),
                new GUIContent("Launch position:"),
                new GUIContent("Target position:"),
                new GUIContent("1"),
                new GUIContent("2"),
                new GUIContent("3"),
                new GUIContent("4"),
                new GUIContent("Damage modifier (%):"),
                new GUIContent("Accuracy modifier (%):"),
                new GUIContent("Critical chance modifier (%):"),
                new GUIContent("Critical damage modifier:"),
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/toolbar_button_edit.png")),
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/toolbar_button_trash.png")),
                new GUIContent("Add effect to list..."),            
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/item_passed_on.png")),
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/item_passed_off.png")),
                new GUIContent("Combine target positions"),
                new GUIContent("Move"),
                new GUIContent("Resistance:"),
            };            

            __heightNormal = EditorGUIUtility.singleLineHeight * 1.5f;
            __colourActive = new Color(0.33f, 0.66f, 1f, 0.82f);
            __colourNormal = new Color(0.43f, 0.43f, 0.43f, 1f);
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Personage/Skill", false, 804)]
        private static void CreateAsset()
        {
            ScriptableHelper.CreateAsset<PuppetSkill>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();

            // --------------------------------------------------------------
            _foldouts[0] = EditorGUILayout.Foldout(_foldouts[0], __content[0], CustomStyles.boldFoldout);
            if (_foldouts[0])
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                UIXHandler();
                EditorGUILayout.EndVertical();
            }            

            // --------------------------------------------------------------
            EditorGUILayout.Space();
            _foldouts[1] = EditorGUILayout.Foldout(_foldouts[1], __content[1], CustomStyles.boldFoldout);
            if (_foldouts[1])
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                PropertiesHandler();
                EditorGUILayout.EndVertical();
            }                        

            // --------------------------------------------------------------
            EditorGUILayout.Space();
            _foldouts[2] = EditorGUILayout.Foldout(_foldouts[2], __content[2], CustomStyles.boldFoldout);
            if (_foldouts[2])
            {
                EditorGUILayout.BeginVertical();
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
            if (effect != null)
            {
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
            else
            {
                _queueOnRemove.Insert(0, index);
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

        public int LaunchPositionEdit(int position, int positionNumber, bool yesNo)
        {
            var number = positionNumber - 1;
            if (number == Mathf.Clamp(number, 0, 3))
            {
                var flag = (1 << number);
                if (yesNo)
                {
                    position |= flag;
                }
                else
                {
                    position &= ~flag;
                }
            }

            return position;
        }

        public bool IsPositionForLaunch(int position, int positionNumber)
        {
            var number = positionNumber - 1;
            if (number == Mathf.Clamp(number, 0, 3))
            {
                return ((position & (1 << number)) != 0);
            }

            return false;
        }

        public int TargetPositionEdit(int position, int positionNumber, bool yesNo)
        {
            var number = positionNumber - 1;
            if (number == Mathf.Clamp(number, 0, 3))
            {
                var flag = ((1 << number) << 4);
                if (yesNo)
                {
                    position |= flag;
                }
                else
                {
                    position &= ~flag;
                }
            }

            return position;
        }

        public bool IsPositionForTarget(int position, int positionNumber)
        {
            var number = positionNumber - 1;
            if (number == Mathf.Clamp(number, 0, 3))
            {
                return ((position & ((1 << number) << 4)) != 0);
            }

            return false;
        }

        private void UIXHandler()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[3], CustomStyles.italicLabel);
            var titleProp = serializedObject.FindProperty("_title");            
            var selectedIndex = Localization.GetKeyIndex(titleProp.stringValue);
            var select = EditorGUILayout.Popup(selectedIndex, Localization.keys);
            if (select != selectedIndex)
            {
                titleProp.stringValue = Localization.GetKey(select);
            }

            EditorGUILayout.Separator();
            var iconProp = serializedObject.FindProperty("_icon");
            iconProp.objectReferenceValue = EditorGUILayout.ObjectField(__content[4], iconProp.objectReferenceValue, typeof(Sprite), false) as Sprite;

            EditorGUILayout.Space();
        }

        private void PropertiesHandler()
        {
            var op1 = GUILayout.Width(32f);
            var op2 = GUILayout.Width(16f);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[23], CustomStyles.italicLabel);
            var resistProp = serializedObject.FindProperty("_resist");
            resistProp.enumValueIndex = (int)((Resists)EditorGUILayout.EnumPopup((Resists)resistProp.enumValueIndex));

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[5], CustomStyles.italicLabel);
            var useToProp = serializedObject.FindProperty("_useTo");
            EditorGUILayout.PropertyField(useToProp, GUIContent.none);

            var positionProp = serializedObject.FindProperty("_position");
            var positionData = positionProp.intValue;

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[6], CustomStyles.italicLabel);            
            EditorGUILayout.BeginHorizontal();
            positionData = LaunchPositionEdit(positionData, 1, EditorGUILayout.ToggleLeft(__content[8],  IsPositionForLaunch(positionData, 1), op1));
            positionData = LaunchPositionEdit(positionData, 2, EditorGUILayout.ToggleLeft(__content[9],  IsPositionForLaunch(positionData, 2), op1));
            positionData = LaunchPositionEdit(positionData, 3, EditorGUILayout.ToggleLeft(__content[10], IsPositionForLaunch(positionData, 3), op1));
            positionData = LaunchPositionEdit(positionData, 4, EditorGUILayout.ToggleLeft(__content[11], IsPositionForLaunch(positionData, 4), op1));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[7], CustomStyles.italicLabel);
            EditorGUILayout.BeginHorizontal();
            positionData = TargetPositionEdit(positionData, 1, EditorGUILayout.ToggleLeft(__content[8],  IsPositionForTarget(positionData, 1), op1));
            positionData = TargetPositionEdit(positionData, 2, EditorGUILayout.ToggleLeft(__content[9],  IsPositionForTarget(positionData, 2), op1));
            positionData = TargetPositionEdit(positionData, 3, EditorGUILayout.ToggleLeft(__content[10], IsPositionForTarget(positionData, 3), op1));
            positionData = TargetPositionEdit(positionData, 4, EditorGUILayout.ToggleLeft(__content[11], IsPositionForTarget(positionData, 4), op1));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            positionProp.intValue = positionData;

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            var combineProp = serializedObject.FindProperty("_combine");
            combineProp.boolValue = EditorGUILayout.ToggleLeft(GUIContent.none, combineProp.boolValue, op2);
            EditorGUILayout.LabelField(__content[21], CustomStyles.italicLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[22], CustomStyles.italicLabel);
            var moveProp = serializedObject.FindProperty("_move");
            moveProp.intValue = EditorGUILayout.IntSlider(moveProp.intValue, -3, 3);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[12], CustomStyles.italicLabel);
            var damageModProp = serializedObject.FindProperty("_damageModifier");
            damageModProp.floatValue = Mathf.Clamp01(EditorGUILayout.Slider(damageModProp.floatValue * 100f, 0f, 100f) * 0.01f);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[13], CustomStyles.italicLabel);
            var accuracyModProp = serializedObject.FindProperty("_accuracyModifier");
            accuracyModProp.floatValue = Mathf.Clamp01(EditorGUILayout.Slider(accuracyModProp.floatValue * 100f, 0f, 100f) * 0.01f);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[14], CustomStyles.italicLabel);
            var critChanceModProp = serializedObject.FindProperty("_critChanceModifier");
            critChanceModProp.floatValue = Mathf.Clamp01(EditorGUILayout.Slider(critChanceModProp.floatValue * 100f, 0f, 100f) * 0.01f);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[15], CustomStyles.italicLabel);
            var critDamageModProp = serializedObject.FindProperty("_critDamageModifier");
            critDamageModProp.floatValue = EditorGUILayout.Slider(critDamageModProp.floatValue, 0f, 100f);

            EditorGUILayout.Space();
        }

        private void EffectsListHandler()
        {
            var op1 = new GUILayoutOption[]
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
            if (GUILayout.Button(__content[16], EditorStyles.toolbarButton, op1))
            {
                EditEffectHandler();
            }
            EditorGUI.EndDisabledGroup();

            var removeDisabled = (_effects.index < 0);
            EditorGUI.BeginDisabledGroup(removeDisabled);
            if (GUILayout.Button(__content[17], EditorStyles.toolbarButton, op1))
            {
                RemoveEffectHandler();
            }
            EditorGUI.EndDisabledGroup();            

            EditorGUILayout.EndHorizontal();

            _effects.DoLayoutList();
            while (_queueOnRemove.Count > 0)
            {
                var removeIndex = _queueOnRemove[0];
                var index = _effects.index;

                _effects.serializedProperty.DeleteArrayElementAtIndex(removeIndex);
                _effects.serializedProperty.DeleteArrayElementAtIndex(removeIndex);

                var prop = serializedObject.FindProperty("_effectsPassed");
                prop.DeleteArrayElementAtIndex(removeIndex);

                serializedObject.ApplyModifiedProperties();

                var upper = _effects.serializedProperty.arraySize;
                _effects.index = Mathf.Min(index, (upper - 1));

                _queueOnRemove.RemoveAt(0);
            }

            var dropArea = GUILayoutUtility.GetRect(0f, 48f, GUILayout.ExpandWidth(true));
            if (EffectsEventHandler(dropArea))
            {
                EditorGUI.DrawRect(dropArea, new Color(0f, 0.75f, 0f, 0.25f));
                EditorGUI.LabelField(dropArea, __content[18], CustomStyles.infoDrop);
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
            _queueOnRemove = new List<int>(PuppetSkill.MaxEffects);
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
