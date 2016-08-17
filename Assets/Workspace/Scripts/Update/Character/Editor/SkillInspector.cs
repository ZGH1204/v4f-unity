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
        private static readonly GUIContent[] __stages = null;
        private static readonly float __heightActive;
        private static readonly float __heightNormal;
        private static readonly Color __colourActive;
        private static readonly Color __colourNormal;
        private static GUIStyle __togglePassedStyle = null;
        private Skill _self = null;
        private SkillStage _stage = null;
        private SerializedObject _stageObj = null;
        private ReorderableList _effects = null;
        private SerializedProperty _effectsPassed = null;
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
                    var tex_on = __content[26].image as Texture2D;
                    var tex_off = __content[27].image as Texture2D;

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
        static SkillInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent("Stage:"),
                GUIContent.none,
                new GUIContent("%"),
                GUIContent.none,
                new GUIContent("Title:"),
                new GUIContent("Description:"),
                new GUIContent("Icon:"),
                new GUIContent("Damage:"),
                new GUIContent("Resistance:"),
                new GUIContent("Goal:"),
                new GUIContent("Launch position:"),
                new GUIContent("Target position:"),
                new GUIContent("1"),
                new GUIContent("2"),
                new GUIContent("3"),
                new GUIContent("4"),
                new GUIContent("Splash"),
                new GUIContent("Move:"),
                new GUIContent("Move chance:"),
                new GUIContent("Stun"),
                new GUIContent("Stun chance:"),
                new GUIContent("Damage modifier:"),
                GUIContent.none,
                new GUIContent("Add effect to list..."),
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/toolbar_button_edit.png")),
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/toolbar_button_trash.png")),
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/item_passed_on.png")),
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/item_passed_off.png")),
            };

            __stages = new GUIContent[Skill.AllStages];
            for (var i = 0; i < Skill.AllStages; ++i)
            {
                __stages[i] = new GUIContent(string.Format("Level {0}", (i + 1)));
            }

            __heightNormal = EditorGUIUtility.singleLineHeight * 1.5f;
            __colourActive = new Color(0.33f, 0.66f, 1f, 0.82f);
            __colourNormal = new Color(0.43f, 0.43f, 0.43f, 1f);
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Character/Skill", false, 803)]
        private static void CreateAsset()
        {
            ScriptableHelper.CreateAsset<Skill>().Initialize();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox(_self.uniqueID, MessageType.None);
            EditorGUILayout.Space();                       

            EditorGUILayout.LabelField(__content[0], CustomStyles.italicLabel);
            var selected = EditorGUILayout.Popup(_self.lastEditStageIndex, __stages);
            if (selected != -1)
            {                                                
                if (selected != _self.lastEditStageIndex)
                {
                    _self.lastEditStageIndex = Setup(selected);
                }

                if (_stage.validate)
                {                    
                    EditorGUILayout.HelpBox("Completed.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Is not prepared!", MessageType.Error);
                }

                _stageObj.Update();

                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                UIXHandler();
                EditorGUILayout.EndVertical();

                EditorGUILayout.Separator();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                PropertiesHandler();
                EditorGUILayout.EndVertical();

                EditorGUILayout.Separator();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EffectsListHandler();
                EditorGUILayout.EndVertical();

                if (GUI.changed)
                {
                    _stageObj.ApplyModifiedProperties();
                }                
            }
            else
            {
                EditorGUILayout.HelpBox("Choose stage for editing.", MessageType.None);
            }            

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_self);
            }            
        }

        private int Setup(int index)
        {
            _stage = serializedObject.FindProperty("_stages").GetArrayElementAtIndex(index).objectReferenceValue as SkillStage;
            _stageObj = new SerializedObject(_stage);

            _effects = MakeEffectsList();
            _effectsPassed = _stageObj.FindProperty("_effectsPassed");
            
            _queueOnRemove.Clear();
            _lastActiveIndex = _effects.index;

            return index;
        }

        private void UIXHandler()
        {
            var titleProp = _stageObj.FindProperty("_title");
            titleProp.stringValue = DrawLocalization(__content[4], titleProp.stringValue);

            EditorGUILayout.Separator();

            var descriptionProp = _stageObj.FindProperty("_description");
            descriptionProp.stringValue = DrawLocalization(__content[5], descriptionProp.stringValue);

            EditorGUILayout.Separator();

            var iconProp = _stageObj.FindProperty("_icon");            
            iconProp.objectReferenceValue = EditorGUILayout.ObjectField(__content[6], iconProp.objectReferenceValue, typeof(Sprite), false) as Sprite;

            EditorGUILayout.Separator();
        }

        private void PropertiesHandler()
        {
            var op1 = GUILayout.Width(24f);
            var op2 = GUILayout.Width(16f);

            EditorGUILayout.LabelField(__content[7], CustomStyles.italicLabel);
            var damageTypeProp = _stageObj.FindProperty("_damageType");
            damageTypeProp.enumValueIndex = (int)((DamageType)EditorGUILayout.EnumPopup((DamageType)damageTypeProp.enumValueIndex));

            EditorGUI.BeginDisabledGroup(!(damageTypeProp.enumValueIndex > 0));
            {
                var damageModifierProp = _stageObj.FindProperty("_damageModifier");
                EditorGUILayout.LabelField(__content[21], CustomStyles.italicLabel);
                EditorGUILayout.BeginHorizontal();
                damageModifierProp.intValue = EditorGUILayout.IntSlider(damageModifierProp.intValue, 0, 200);
                EditorGUILayout.LabelField(__content[2], op2);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField(__content[8], CustomStyles.italicLabel);
            var resistTypeProp = _stageObj.FindProperty("_resistType");
            resistTypeProp.enumValueIndex = (int)((ResistanceType)EditorGUILayout.EnumPopup((ResistanceType)resistTypeProp.enumValueIndex));

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField(__content[9], CustomStyles.italicLabel);
            var goalProp = _stageObj.FindProperty("_goal");
            goalProp.enumValueIndex = (int)((SkillGoal)EditorGUILayout.EnumPopup((SkillGoal)goalProp.enumValueIndex));

            EditorGUILayout.Separator();

            var positionProp = _stageObj.FindProperty("_position");
            var positionData = positionProp.intValue;
            {
                EditorGUILayout.LabelField(__content[10], CustomStyles.italicLabel);
                EditorGUILayout.BeginHorizontal();
                positionData = LaunchPositionEdit(positionData, 1, EditorGUILayout.ToggleLeft(__content[12], IsPositionForLaunch(positionData, 1), op1));
                positionData = LaunchPositionEdit(positionData, 2, EditorGUILayout.ToggleLeft(__content[13], IsPositionForLaunch(positionData, 2), op1));
                positionData = LaunchPositionEdit(positionData, 3, EditorGUILayout.ToggleLeft(__content[14], IsPositionForLaunch(positionData, 3), op1));
                positionData = LaunchPositionEdit(positionData, 4, EditorGUILayout.ToggleLeft(__content[15], IsPositionForLaunch(positionData, 4), op1));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Separator();

                EditorGUILayout.LabelField(__content[11], CustomStyles.italicLabel);
                EditorGUILayout.BeginHorizontal();
                positionData = TargetPositionEdit(positionData, 1, EditorGUILayout.ToggleLeft(__content[12], IsPositionForTarget(positionData, 1), op1));
                positionData = TargetPositionEdit(positionData, 2, EditorGUILayout.ToggleLeft(__content[13], IsPositionForTarget(positionData, 2), op1));
                positionData = TargetPositionEdit(positionData, 3, EditorGUILayout.ToggleLeft(__content[14], IsPositionForTarget(positionData, 3), op1));
                positionData = TargetPositionEdit(positionData, 4, EditorGUILayout.ToggleLeft(__content[15], IsPositionForTarget(positionData, 4), op1));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            positionProp.intValue = positionData;            

            var splashProp = _stageObj.FindProperty("_splash");
            EditorGUILayout.BeginHorizontal();
            splashProp.boolValue = EditorGUILayout.ToggleLeft(__content[1], splashProp.boolValue, op2);
            EditorGUILayout.LabelField(__content[16], CustomStyles.italicLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            var moveProp = _stageObj.FindProperty("_move");
            EditorGUILayout.LabelField(__content[17], CustomStyles.italicLabel);
            moveProp.intValue = EditorGUILayout.IntSlider(moveProp.intValue, -3, 3);

            EditorGUI.BeginDisabledGroup(moveProp.intValue == 0);
            {
                var moveChanceProp = _stageObj.FindProperty("_moveСhance");
                EditorGUILayout.LabelField(__content[18], CustomStyles.italicLabel);
                EditorGUILayout.BeginHorizontal();
                moveChanceProp.intValue = EditorGUILayout.IntSlider(moveChanceProp.intValue, 0, 100);
                EditorGUILayout.LabelField(__content[2], op2);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Separator();

            var stunProp = _stageObj.FindProperty("_stun");
            EditorGUILayout.BeginHorizontal();
            stunProp.boolValue = EditorGUILayout.ToggleLeft(__content[1], stunProp.boolValue, op2);
            EditorGUILayout.LabelField(__content[19], CustomStyles.italicLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!stunProp.boolValue);
            {
                var stunChanceProp = _stageObj.FindProperty("_stunСhance");
                EditorGUILayout.LabelField(__content[20], CustomStyles.italicLabel);
                EditorGUILayout.BeginHorizontal();
                stunChanceProp.intValue = EditorGUILayout.IntSlider(stunChanceProp.intValue, 0, 100);
                EditorGUILayout.LabelField(__content[2], op2);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Separator();
        }

        private void EffectsListHandler()
        {
            var op1 = new GUILayoutOption[]
            {
                GUILayout.MinWidth(42f),
                GUILayout.MaxWidth(42f),
            };

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            {
                var count = _stage.countEffects;
                var stats = string.Format("count: {0} / {1}", count, SkillStage.MaxEffects);
                EditorGUILayout.LabelField(stats, CustomStyles.italicLabel, GUILayout.Width(96f));

                GUILayout.FlexibleSpace();

                var editingDisabled = (_effects.index < 0);
                EditorGUI.BeginDisabledGroup(editingDisabled);
                if (GUILayout.Button(__content[24], EditorStyles.toolbarButton, op1))
                {
                    EditEffectHandler();
                }
                EditorGUI.EndDisabledGroup();

                var removeDisabled = (_effects.index < 0);
                EditorGUI.BeginDisabledGroup(removeDisabled);
                if (GUILayout.Button(__content[25], EditorStyles.toolbarButton, op1))
                {
                    RemoveEffectHandler();
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            _effects.DoLayoutList();
            while (_queueOnRemove.Count > 0)
            {
                var removeIndex = _queueOnRemove[0];
                var index = _effects.index;

                _effects.serializedProperty.DeleteArrayElementAtIndex(removeIndex);
                _effects.serializedProperty.DeleteArrayElementAtIndex(removeIndex);

                _effectsPassed.DeleteArrayElementAtIndex(removeIndex);                

                var upper = _effects.serializedProperty.arraySize;
                _effects.index = Mathf.Min(index, (upper - 1));

                _queueOnRemove.RemoveAt(0);
                _stageObj.ApplyModifiedProperties();
            }

            var dropArea = GUILayoutUtility.GetRect(0f, 48f, GUILayout.ExpandWidth(true));
            if (EffectsEventHandler(dropArea))
            {
                EditorGUI.DrawRect(dropArea, new Color(0f, 0.75f, 0f, 0.25f));
                EditorGUI.LabelField(dropArea, __content[23], CustomStyles.infoDrop);
            }

            EditorGUILayout.Separator();
        }

        private string DrawLocalization(GUIContent content, string key)
        {
            EditorGUILayout.LabelField(content, CustomStyles.italicLabel);

            var selectedIndex = Localization.GetKeyIndex(key);
            var select = EditorGUILayout.Popup(selectedIndex, Localization.keys);
            if (select != selectedIndex)
            {
                return Localization.GetKey(select);
            }

            return key;
        }

        private int LaunchPositionEdit(int position, int positionNumber, bool yesNo)
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

        private bool IsPositionForLaunch(int position, int positionNumber)
        {
            var number = positionNumber - 1;
            if (number == Mathf.Clamp(number, 0, 3))
            {
                return ((position & (1 << number)) != 0);
            }

            return false;
        }

        private int TargetPositionEdit(int position, int positionNumber, bool yesNo)
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

        private bool IsPositionForTarget(int position, int positionNumber)
        {
            var number = positionNumber - 1;
            if (number == Mathf.Clamp(number, 0, 3))
            {
                return ((position & ((1 << number) << 4)) != 0);
            }

            return false;
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
            
            var effect = _effects.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue as Effect;
            if (effect != null)
            {                
                var property = _effectsPassed.GetArrayElementAtIndex(index);

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

                EditorGUI.LabelField(rcTitle, effect.name, titleStyle);
                
                EditorGUI.BeginChangeCheck();
                property.boolValue = EditorGUI.Toggle(rcPassed, GUIContent.none, property.boolValue, togglePassedStyle);
                if (EditorGUI.EndChangeCheck())
                {
                    _stageObj.ApplyModifiedProperties();
                }                
            }
            else
            {
                _queueOnRemove.Insert(0, index);
            }
        }

        private ReorderableList MakeEffectsList()
        {
            var elements = _stageObj.FindProperty("_effects");
            var reorderList = new ReorderableList(_stageObj, elements, true, false, false, false);

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
                    var effect = property.objectReferenceValue as Effect;
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

                var item = _effectsPassed.GetArrayElementAtIndex(_lastActiveIndex);
                var temp = item.boolValue;

                _effectsPassed.DeleteArrayElementAtIndex(_lastActiveIndex);
                _effectsPassed.InsertArrayElementAtIndex(index);

                item = _effectsPassed.GetArrayElementAtIndex(index);
                item.boolValue = temp;

                _stageObj.ApplyModifiedProperties();
            };            

            return reorderList;
        }

        private void EditEffectHandler()
        {            
            var property = _effects.serializedProperty.GetArrayElementAtIndex(_effects.index);
            var effect = property.objectReferenceValue as Effect;

            Selection.activeObject = effect;
            EditorGUIUtility.PingObject(effect);
        }

        private void RemoveEffectHandler()
        {
            var index = _effects.index;

            _effects.serializedProperty.DeleteArrayElementAtIndex(index);
            _effects.serializedProperty.DeleteArrayElementAtIndex(index);

            _effectsPassed.DeleteArrayElementAtIndex(index);            

            var upper = _effects.serializedProperty.arraySize;
            _effects.index = Mathf.Min(index, (upper - 1));

            _stageObj.ApplyModifiedProperties();
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
                if ((_stage.countEffects < SkillStage.MaxEffects) && dropArea.Contains(currentEvent.mousePosition))
                {
                    _drawDropArea = true;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (eventType == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            Effect effect = draggedObject as Effect;
                            if (effect != null)
                            {                                
                                var index = _effects.serializedProperty.arraySize;
                                _effects.serializedProperty.InsertArrayElementAtIndex(index);
                                _effects.index = index;

                                var prop = _effects.serializedProperty.GetArrayElementAtIndex(index);
                                prop.objectReferenceValue = effect;
                                
                                _effectsPassed.InsertArrayElementAtIndex(index);
                                _effectsPassed.GetArrayElementAtIndex(index).boolValue = true;

                                _lastActiveIndex = _effects.index;
                                _stageObj.ApplyModifiedProperties();
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
            _queueOnRemove = new List<int>(SkillStage.MaxEffects);
            _drawDropArea = false;

            _self = target as Skill;
            if (_self.lastEditStageIndex != -1)
            {
                Setup(_self.lastEditStageIndex);
            }                       
        }
        #endregion
    }

}
