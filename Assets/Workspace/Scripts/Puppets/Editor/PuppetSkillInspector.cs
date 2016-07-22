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
        private static readonly GUIContent  __toggleEditing = null;
        private static readonly GUIContent  __buttonAddEffect = null;
        private static readonly GUIContent  __buttonDelEffect = null;
        private static readonly GUIContent  __effectTitle;
        private static readonly GUIContent  __effectTimer;
        private static readonly GUIContent  __effectMinDamage;
        private static readonly GUIContent  __effectMaxDamage;
        private static readonly GUIContent  __effectApplyDamage;
        private static readonly GUIContent  __effectInvertDamage;
        private static readonly GUIContent  __effectStun;
        private static readonly float       __heightActive;
        private static readonly float       __heightNormal;
        private static readonly Color       __colourActive;
        private static readonly Color       __colourNormal;

        private static GUIStyle __foldoutStyle = null;
        private static GUIStyle __subLabelStyle = null;        

        private PuppetSkill _self;
        private bool[] _foldouts;
        private ReorderableList _effects;
        #endregion

        #region Properties
        private static GUIStyle foldoutStyle
        {
            get
            {
                if (__foldoutStyle == null)
                {
                    __foldoutStyle = new GUIStyle(EditorStyles.foldout);
                    __foldoutStyle.fontStyle = FontStyle.Bold;
                }

                return __foldoutStyle;
            }
        }

        private static GUIStyle subLabelStyle
        {
            get
            {
                if (__subLabelStyle == null)
                {
                    __subLabelStyle = new GUIStyle(EditorStyles.label);
                    __subLabelStyle.fontStyle = FontStyle.Italic;
                }

                return __subLabelStyle;
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
            __toggleEditing = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/toolbar_button_edit.png"));
            __buttonAddEffect = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/toolbar_button_add.png"));
            __buttonDelEffect = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/toolbar_button_trash.png"));
            __effectTitle = new GUIContent("Name:");
            __effectTimer = new GUIContent("Timer:");
            __effectMinDamage = new GUIContent("Мinimum damage:");
            __effectMaxDamage = new GUIContent("Мaximum damage:");
            __effectApplyDamage = new GUIContent("Apply damage");
            __effectInvertDamage = new GUIContent("Invert");
            __effectStun = new GUIContent("Stun");
            __heightActive = EditorGUIUtility.singleLineHeight * 13.5f;
            __heightNormal = EditorGUIUtility.singleLineHeight * 1.25f;
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
            _foldouts[0] = EditorGUILayout.Foldout(_foldouts[0], __labelUIX, foldoutStyle);
            if (_foldouts[0])
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                UIXHandler();
                EditorGUILayout.EndVertical();
            }            

            // --------------------------------------------------------------
            EditorGUILayout.Space();
            _foldouts[1] = EditorGUILayout.Foldout(_foldouts[1], __labelProperties, foldoutStyle);
            if (_foldouts[1])
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                PropertiesHandler();
                EditorGUILayout.EndVertical();
            }                        

            // --------------------------------------------------------------
            EditorGUILayout.Space();
            _foldouts[2] = EditorGUILayout.Foldout(_foldouts[2], __labelEffects, foldoutStyle);
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

            return reorderList;
        }

        private float ElementHeight(int index)
        {
            return (((_effects.index == index) && !_effects.draggable) ? __heightActive : __heightNormal);
        }

        private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index < 0)
            {
                return;
            }

            rect.height = ((isActive && !_effects.draggable) ? __heightActive : __heightNormal);

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

            if (isActive && !_effects.draggable)
            {
                var offset = 4f;

                var rcTitle = new Rect(rect.x + 4f, rect.y + offset, rect.width - 8f, EditorGUIUtility.singleLineHeight);
                offset += rcTitle.height + 2f;
                var rcTitleField = new Rect(rect.x + 4f, rect.y + offset, rect.width - 8f, EditorGUIUtility.singleLineHeight);
                offset += rcTitleField.height + 4f;
                var rcTimer = new Rect(rect.x + 4f, rect.y + offset, rect.width - 8f, EditorGUIUtility.singleLineHeight);
                offset += rcTimer.height + 2f;
                var rcTimerField = new Rect(rect.x + 4f, rect.y + offset, rect.width - 8f, EditorGUIUtility.singleLineHeight);
                offset += rcTimerField.height + 4f;
                var rcApplyToggle = new Rect(rect.x + 4f, rect.y + offset, 16f, EditorGUIUtility.singleLineHeight);
                var rcApply = new Rect(rcApplyToggle.x + rcApplyToggle.width + 2f, rect.y + offset, rect.width - rcApplyToggle.width - 8f, EditorGUIUtility.singleLineHeight);
                offset += rcApply.height + 2f;
                var rcInvertToggle = new Rect(rect.x + 12f, rect.y + offset, 16f, EditorGUIUtility.singleLineHeight);
                var rcInvert = new Rect(rcInvertToggle.x + rcInvertToggle.width + 2f, rect.y + offset, rect.width - rcInvertToggle.width - 16f, EditorGUIUtility.singleLineHeight);
                offset += rcInvert.height + 2f;
                var rcMin = new Rect(rect.x + 12f, rect.y + offset, rect.width - 16f, EditorGUIUtility.singleLineHeight);
                offset += rcMin.height + 2f;
                var rcMinField = new Rect(rect.x + 12f, rect.y + offset, rect.width - 16f, EditorGUIUtility.singleLineHeight);
                offset += rcMinField.height + 2f;
                var rcMax = new Rect(rect.x + 12f, rect.y + offset, rect.width - 16f, EditorGUIUtility.singleLineHeight);
                offset += rcMax.height + 2f;
                var rcMaxField = new Rect(rect.x + 12f, rect.y + offset, rect.width - 16f, EditorGUIUtility.singleLineHeight);
                offset += rcMaxField.height + 4f;
                var rcStunToggle = new Rect(rect.x + 4f, rect.y + offset, 16f, EditorGUIUtility.singleLineHeight);
                var rcStun = new Rect(rcStunToggle.x + rcStunToggle.width + 2f, rect.y + offset, rect.width - rcStunToggle.width - 8f, EditorGUIUtility.singleLineHeight);
                offset += rcStun.height + 4f;


                EditorGUI.LabelField(rcTitle, __effectTitle, subLabelStyle);
                var title = Regex.Replace(EditorGUI.DelayedTextField(rcTitleField, effect.title), @"[^a-zA-Z0-9_ ]", "").Trim();
                if (!string.IsNullOrEmpty(title) && (title.Length > 3))
                {
                    effect.title = title;
                }

                EditorGUI.LabelField(rcTimer, __effectTimer, subLabelStyle);
                effect.timer = EditorGUI.IntSlider(rcTimerField, effect.timer, 0, 10);

                effect.applyDamage = EditorGUI.ToggleLeft(rcApplyToggle, GUIContent.none, effect.applyDamage);
                EditorGUI.LabelField(rcApply, __effectApplyDamage, subLabelStyle);

                EditorGUI.BeginDisabledGroup(!effect.applyDamage);

                effect.invertDamage = EditorGUI.ToggleLeft(rcInvertToggle, GUIContent.none, effect.invertDamage);
                EditorGUI.LabelField(rcInvert, __effectInvertDamage, subLabelStyle);

                EditorGUI.LabelField(rcMin, __effectMinDamage, subLabelStyle);
                effect.minDamage = EditorGUI.IntSlider(rcMinField, effect.minDamage, 0, effect.maxDamage);

                EditorGUI.LabelField(rcMax, __effectMaxDamage, subLabelStyle);
                effect.maxDamage = EditorGUI.IntSlider(rcMaxField, effect.maxDamage, effect.minDamage, 100);

                EditorGUI.EndDisabledGroup();

                effect.stun = EditorGUI.ToggleLeft(rcStunToggle, GUIContent.none, effect.stun);
                EditorGUI.LabelField(rcStun, __effectStun, subLabelStyle);
            }
            else
            {
                var rcTitle = new Rect(rect.x + 4, rect.y, rect.width - 8, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(rcTitle, effect.title);
            }            
        }

        private void AddEffectHandler()
        {
            var index = _effects.serializedProperty.arraySize;

            _self.AddEffect();
            serializedObject.ApplyModifiedProperties();

            _effects.index = index;
        }

        private void DelEffectHandler()
        {
            var index = _effects.index;

            var property = _effects.serializedProperty.GetArrayElementAtIndex(index);
            DestroyImmediate(property.objectReferenceValue as PuppetEffect);

            _effects.serializedProperty.DeleteArrayElementAtIndex(index);
            _effects.serializedProperty.DeleteArrayElementAtIndex(index);

            serializedObject.ApplyModifiedProperties();

            var upper = _effects.serializedProperty.arraySize;            
            _effects.index = Mathf.Min(index, (upper - 1));            
        }

        private void UIXHandler()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelTitle, subLabelStyle);
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
            EditorGUILayout.LabelField(__paramUseOnTarget, subLabelStyle);
            _self.useOnTarget = (PuppetSkillTarget)EditorGUILayout.EnumPopup(_self.useOnTarget);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelTargetsQuantity, subLabelStyle);
            _self.targetsQuantity = EditorGUILayout.IntSlider(_self.targetsQuantity, 1, 4);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelСanUseInPositions, subLabelStyle);
            EditorGUILayout.BeginHorizontal();
            _self.UseInPosition(1, EditorGUILayout.ToggleLeft(__paramPosition1, _self.CanUseInPosition(1), GUILayout.Width(32f)));
            _self.UseInPosition(2, EditorGUILayout.ToggleLeft(__paramPosition2, _self.CanUseInPosition(2), GUILayout.Width(32f)));
            _self.UseInPosition(3, EditorGUILayout.ToggleLeft(__paramPosition3, _self.CanUseInPosition(3), GUILayout.Width(32f)));
            _self.UseInPosition(4, EditorGUILayout.ToggleLeft(__paramPosition4, _self.CanUseInPosition(4), GUILayout.Width(32f)));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelСanUseToTargetPositions, subLabelStyle);
            EditorGUILayout.BeginHorizontal();
            _self.UseToTargetPosition(1, EditorGUILayout.ToggleLeft(__paramPosition1, _self.CanUseToTargetPosition(1), GUILayout.Width(32f)));
            _self.UseToTargetPosition(2, EditorGUILayout.ToggleLeft(__paramPosition2, _self.CanUseToTargetPosition(2), GUILayout.Width(32f)));
            _self.UseToTargetPosition(3, EditorGUILayout.ToggleLeft(__paramPosition3, _self.CanUseToTargetPosition(3), GUILayout.Width(32f)));
            _self.UseToTargetPosition(4, EditorGUILayout.ToggleLeft(__paramPosition4, _self.CanUseToTargetPosition(4), GUILayout.Width(32f)));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelDamageModifier, subLabelStyle);
            _self.damageModifier = EditorGUILayout.Slider(_self.damageModifier * 100f, 0f, 100f) * 0.01f;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelAccuracyModifier, subLabelStyle);
            _self.accuracyModifier = EditorGUILayout.Slider(_self.accuracyModifier * 100f, 0f, 100f) * 0.01f;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelCritChanceModifier, subLabelStyle);
            _self.critChanceModifier = EditorGUILayout.Slider(_self.critChanceModifier * 100f, 0f, 100f) * 0.01f;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(__labelCritDamageModifier, subLabelStyle);
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

            var editingDisabled = (_effects.index < 0);
            EditorGUI.BeginDisabledGroup(editingDisabled);
            _effects.draggable = !GUILayout.Toggle(!_effects.draggable, __toggleEditing, EditorStyles.toolbarButton, toolbarButtonOp);
            EditorGUI.EndDisabledGroup();

            GUILayout.FlexibleSpace();

            var addingDisabled = false;
            EditorGUI.BeginDisabledGroup(addingDisabled);
            if (GUILayout.Button(__buttonAddEffect, EditorStyles.toolbarButton, toolbarButtonOp))
            {
                AddEffectHandler();
            }
            EditorGUI.EndDisabledGroup();

            var removeDisabled = (_effects.index < 0);
            EditorGUI.BeginDisabledGroup(removeDisabled);
            if (GUILayout.Button(__buttonDelEffect, EditorStyles.toolbarButton, toolbarButtonOp))
            {
                DelEffectHandler();
            }
            EditorGUI.EndDisabledGroup();            

            EditorGUILayout.EndHorizontal();

            _effects.DoLayoutList();

            EditorGUILayout.Space();
        }        

        private void OnEnable()
        {
            _self = target as PuppetSkill;
            _foldouts = new bool[] { true, true, true };
            _effects = MakeEffectsList();
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
