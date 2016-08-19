// <copyright file="AttributeInfoInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEditor;

using V4F.Character;

namespace V4F.UI.Valhalla
{

    [CustomEditor(typeof(AttributeInfo)), InitializeOnLoad]
    public class AttributeInfoInspector : Editor
    {
        private static readonly GUIContent[] __content = null;
        private static readonly AttributeType[] __attributes = null;
        private AttributeInfo _self;

        static AttributeInfoInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent("Statistics"),
                new GUIContent("Attribute"),
                new GUIContent("Title UI"),
                new GUIContent("Value UI"),
                new GUIContent("Title"),
            };

            __attributes = System.Enum.GetValues(typeof(AttributeType)) as AttributeType[];
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var statsProp = serializedObject.FindProperty("_stats");
            EditorGUILayout.PropertyField(statsProp, __content[0]);

            var typeProp = serializedObject.FindProperty("_type");            
            typeProp.enumValueIndex = (int)((AttributeType)EditorGUILayout.EnumPopup(__content[1], __attributes[typeProp.enumValueIndex]));

            var titleUIProp = serializedObject.FindProperty("_titleUI");
            EditorGUILayout.PropertyField(titleUIProp, __content[2]);

            var valueUIProp = serializedObject.FindProperty("_valueUI");
            EditorGUILayout.PropertyField(valueUIProp, __content[3]);

            var titleProp = serializedObject.FindProperty("_title");
            titleProp.stringValue = DrawLocalization(__content[4], titleProp.stringValue);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_self);
            }
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

        private void OnEnable()
        {
            _self = target as AttributeInfo;
        }
    }
	
}
