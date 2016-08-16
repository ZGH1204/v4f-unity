// <copyright file="SpecificationInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEditor;

namespace V4F.Character
{

    [CustomEditor(typeof(Specification)), InitializeOnLoad]
    public class SpecificationInspector : Editor
    {
        #region Fields
        private static readonly GUIContent[] __content = null;
        private Specification _self = null;
        private bool[] _foldout = null;
        #endregion

        #region Constructors
        static SpecificationInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent("%"),
                new GUIContent("Base"),
                new GUIContent("Factor"),
                GUIContent.none,
                new GUIContent("Character attributes"),
                new GUIContent("Strength"),
                new GUIContent("Dexterity"),
                new GUIContent("Magic"),
                new GUIContent("Vitality"),
                new GUIContent("Health points"),
                new GUIContent("Min damage (melee)"),
                new GUIContent("Max damage (melee)"),
                new GUIContent("Min damage (range)"),
                new GUIContent("Max damage (range)"),
                new GUIContent("Min damage (magic)"),
                new GUIContent("Max damage (magic)"),
                new GUIContent("Chance to dodge"),
                new GUIContent("Chance to crit"),
                GUIContent.none,
                new GUIContent("Resistances"),
                new GUIContent("Fire"),
                new GUIContent("Ice"),
                new GUIContent("Lighting"),
                new GUIContent("Death"),
                new GUIContent("Poison"),
                new GUIContent("Stun/Move"),
                new GUIContent("Immune"),
                new GUIContent("Bleeding"),
            };
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Character/Specification", false, 800)]
        private static void CreateAsset()
        {
            ScriptableHelper.CreateAsset<Specification>().Initialize();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var attributes = serializedObject.FindProperty("_attributes");
            var factors = serializedObject.FindProperty("_factors");

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField(__content[4], EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.Space();

                    // BASE --------------------------------------------------------------------------------
                    var strength = attributes.GetArrayElementAtIndex(0);
                    strength.intValue = DrawIntAttribute(__content[5], strength.intValue, 1, 100, false);
                    EditorGUILayout.Separator();
                    var dexterity = attributes.GetArrayElementAtIndex(1);
                    dexterity.intValue = DrawIntAttribute(__content[6], dexterity.intValue, 1, 100, false);
                    EditorGUILayout.Separator();
                    var magic = attributes.GetArrayElementAtIndex(2);
                    magic.intValue = DrawIntAttribute(__content[7], magic.intValue, 1, 100, false);
                    EditorGUILayout.Separator();
                    var vitality = attributes.GetArrayElementAtIndex(3);
                    vitality.intValue = DrawIntAttribute(__content[8], vitality.intValue, 1, 100, false);
                    //--------------------------------------------------------------------------------------

                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();

                    //DESIGN -------------------------------------------------------------------------------                                        
                    var bo_value = 0;
                    var fo_value = 0f;

                    var bi_value = attributes.GetArrayElementAtIndex(4); // health points
                    var fi_value = factors.GetArrayElementAtIndex(4);                    
                    DrawBaseFactorAttribute(0, __content[9], bi_value.intValue, 0, 100, fi_value.floatValue, out bo_value, out fo_value);
                    bi_value.intValue = bo_value;
                    fi_value.floatValue = fo_value;

                    EditorGUILayout.Separator();
                    var min_bi_value = attributes.GetArrayElementAtIndex(5); // min damage melee
                    var max_bi_value = attributes.GetArrayElementAtIndex(6); // max damage melee                                                                             
                    fi_value = factors.GetArrayElementAtIndex(6);
                    DrawBaseFactorAttribute(1, __content[11], max_bi_value.intValue, min_bi_value.intValue, 100, fi_value.floatValue, out bo_value, out fo_value);
                    max_bi_value.intValue = bo_value;
                    fi_value.floatValue = fo_value;
                    EditorGUILayout.Separator();
                    fi_value = factors.GetArrayElementAtIndex(5);
                    DrawBaseFactorAttribute(2, __content[10], min_bi_value.intValue, 0, max_bi_value.intValue, fi_value.floatValue, out bo_value, out fo_value);
                    min_bi_value.intValue = bo_value;
                    fi_value.floatValue = fo_value;

                    EditorGUILayout.Separator();
                    min_bi_value = attributes.GetArrayElementAtIndex(7); // min damage rabge
                    max_bi_value = attributes.GetArrayElementAtIndex(8); // max damage range
                    fi_value = factors.GetArrayElementAtIndex(8);
                    DrawBaseFactorAttribute(3, __content[13], max_bi_value.intValue, min_bi_value.intValue, 100, fi_value.floatValue, out bo_value, out fo_value);
                    max_bi_value.intValue = bo_value;
                    fi_value.floatValue = fo_value;
                    EditorGUILayout.Separator();
                    fi_value = factors.GetArrayElementAtIndex(7);
                    DrawBaseFactorAttribute(4, __content[12], min_bi_value.intValue, 0, max_bi_value.intValue, fi_value.floatValue, out bo_value, out fo_value);
                    min_bi_value.intValue = bo_value;
                    fi_value.floatValue = fo_value;

                    EditorGUILayout.Separator();
                    min_bi_value = attributes.GetArrayElementAtIndex(9); // min damage magic
                    max_bi_value = attributes.GetArrayElementAtIndex(10); // max damage magic                                        
                    fi_value = factors.GetArrayElementAtIndex(10);
                    DrawBaseFactorAttribute(5, __content[15], max_bi_value.intValue, min_bi_value.intValue, 100, fi_value.floatValue, out bo_value, out fo_value);
                    max_bi_value.intValue = bo_value;
                    fi_value.floatValue = fo_value;
                    EditorGUILayout.Separator();
                    fi_value = factors.GetArrayElementAtIndex(9);
                    DrawBaseFactorAttribute(6, __content[14], min_bi_value.intValue, 0, max_bi_value.intValue, fi_value.floatValue, out bo_value, out fo_value);
                    min_bi_value.intValue = bo_value;
                    fi_value.floatValue = fo_value;

                    EditorGUILayout.Separator();
                    bi_value = attributes.GetArrayElementAtIndex(11); // chance to dodge
                    fi_value = factors.GetArrayElementAtIndex(11);
                    DrawBaseFactorAttribute(7, __content[16], bi_value.intValue, 0, 100, fi_value.floatValue, out bo_value, out fo_value, true);
                    bi_value.intValue = bo_value;
                    fi_value.floatValue = fo_value;

                    EditorGUILayout.Separator();
                    bi_value = attributes.GetArrayElementAtIndex(12); // chance to crit
                    fi_value = factors.GetArrayElementAtIndex(12);
                    DrawBaseFactorAttribute(8, __content[17], bi_value.intValue, 0, 100, fi_value.floatValue, out bo_value, out fo_value, true);
                    bi_value.intValue = bo_value;
                    fi_value.floatValue = fo_value;
                    //--------------------------------------------------------------------------------------

                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();

                EditorGUILayout.LabelField(__content[19], EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    var fireResist = attributes.GetArrayElementAtIndex(13);
                    fireResist.intValue = DrawIntAttribute(__content[20], fireResist.intValue, 0, 100, true);
                    EditorGUILayout.Separator();
                    var iceResist = attributes.GetArrayElementAtIndex(14);
                    iceResist.intValue = DrawIntAttribute(__content[21], iceResist.intValue, 0, 100, true);
                    EditorGUILayout.Separator();
                    var lightingResist = attributes.GetArrayElementAtIndex(15);
                    lightingResist.intValue = DrawIntAttribute(__content[22], lightingResist.intValue, 0, 100, true);
                    EditorGUILayout.Separator();
                    var deathResist = attributes.GetArrayElementAtIndex(16);
                    deathResist.intValue = DrawIntAttribute(__content[23], deathResist.intValue, 0, 100, true);
                    EditorGUILayout.Separator();
                    var poisonResist = attributes.GetArrayElementAtIndex(17);
                    poisonResist.intValue = DrawIntAttribute(__content[24], poisonResist.intValue, 0, 100, true);
                    EditorGUILayout.Separator();
                    var stunMoveResist = attributes.GetArrayElementAtIndex(18);
                    stunMoveResist.intValue = DrawIntAttribute(__content[25], stunMoveResist.intValue, 0, 100, true);
                    EditorGUILayout.Separator();
                    var immuneResist = attributes.GetArrayElementAtIndex(19);
                    immuneResist.intValue = DrawIntAttribute(__content[26], immuneResist.intValue, 0, 100, true);
                    EditorGUILayout.Separator();
                    var bleedingResist = attributes.GetArrayElementAtIndex(20);
                    bleedingResist.intValue = DrawIntAttribute(__content[27], bleedingResist.intValue, 0, 100, true);
                }
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_self);
            }
        }

        private int DrawIntAttribute(GUIContent content, int value, int min, int max, bool isPercent)
        {
            var op1 = new GUILayoutOption[] { GUILayout.Width(64f), GUILayout.MinWidth(64f), GUILayout.MaxWidth(64f) };
            var op2 = new GUILayoutOption[] { GUILayout.Width(16f), GUILayout.MinWidth(16f), GUILayout.MaxWidth(16f) };

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(content, CustomStyles.italicLabel, op1);
                value = EditorGUILayout.IntSlider(value, min, max);
                var percent = (isPercent ? __content[0] : GUIContent.none);
                EditorGUILayout.LabelField(percent, op2);
            }
            EditorGUILayout.EndHorizontal();

            return value;
        }

        private float DrawFloatAttribute(GUIContent content, float value, float min, float max, bool isPercent)
        {
            var op1 = new GUILayoutOption[] { GUILayout.Width(64f), GUILayout.MinWidth(64f), GUILayout.MaxWidth(64f) };
            var op2 = new GUILayoutOption[] { GUILayout.Width(16f), GUILayout.MinWidth(16f), GUILayout.MaxWidth(16f) };

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(content, CustomStyles.italicLabel, op1);
                value = EditorGUILayout.Slider(value, min, max);
                var percent = (isPercent ? __content[0] : __content[3]);
                EditorGUILayout.LabelField(percent, op2);
            }
            EditorGUILayout.EndHorizontal();

            return value;
        }

        private void DrawBaseFactorAttribute(int index, GUIContent content, int baseIn, int min, int max, float factorIn, out int baseOut, out float factorOut, bool isPercent = false)
        {            
            var op1 = GUILayout.Width(16f);
            var op2 = GUILayout.Width(8f);

            baseOut = baseIn;
            factorOut = factorIn;

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(__content[3], op2);
                    _foldout[index] = EditorGUILayout.Foldout(_foldout[index], content, CustomStyles.italicFoldout);
                }
                EditorGUILayout.EndHorizontal();

                if (_foldout[index])
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(__content[3], op1);
                        baseOut = DrawIntAttribute(__content[1], baseIn, min, max, isPercent);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(__content[3], op1);
                        factorOut = DrawFloatAttribute(__content[2], factorIn, -100f, 100f, false);
                    }
                    EditorGUILayout.EndHorizontal();
                }                
            }
            EditorGUILayout.EndVertical();
        }

        private void OnEnable()
        {
            _self = target as Specification;
            _foldout = new bool[32];
        }
        #endregion
    }

}
