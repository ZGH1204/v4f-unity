// <copyright file="PuppetResistsInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEditor;

namespace V4F.Puppets
{

    [CustomEditor(typeof(PuppetResists)), InitializeOnLoad]
    public class PuppetResistsInspector : Editor
    {
        #region Fields
        private static readonly GUIContent[] __content = null;
        private static readonly Resists[] __resists = null;

        private PuppetResists _self;
        #endregion

        #region Constructors
        static PuppetResistsInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent("Magic of Elements (%)"),
                new GUIContent("Magic of Nature (%)"),
                new GUIContent("Magic of Death (%)"),
                new GUIContent("Disease (%)"),
                new GUIContent("Stun (%)"),
                new GUIContent("Bleed (%)"),
                new GUIContent("Move (%)"),                
            };

            __resists = System.Enum.GetValues(typeof(Resists)) as Resists[];
        }
        #endregion

        #region Methods
        //[MenuItem("Assets/Create/V4F/Personage/Resistances", false, 801)]
        private static void CreateAsset()
        {
            var asset = ScriptableHelper.CreateAsset<PuppetResists>();
            asset.Initialize();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    var count = __resists.Length - 1;
                    for (int i = 0; i < count; ++i)
                    {
                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField(__content[__resists[i].GetIndex()], CustomStyles.italicLabel);
                        _self.SetResist(__resists[i], EditorGUILayout.IntSlider(_self.GetResist(__resists[i]), 0, 100));
                    }                    

                    EditorGUILayout.Space();
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

        private void OnEnable()
        {
            _self = target as PuppetResists;
        }
        #endregion
    }

}
