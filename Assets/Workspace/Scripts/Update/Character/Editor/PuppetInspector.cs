// <copyright file="PuppetInspector.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEditor;

namespace V4F.Character
{

    [CustomEditor(typeof(Puppet)), InitializeOnLoad]
    public class PuppetInspector : Editor
    {
        #region Fields
        private static readonly GUIContent[] __content = null;
        private Puppet _self = null;
        #endregion

        #region Constructors
        static PuppetInspector()
        {
            __content = new GUIContent[]
            {
                new GUIContent("Editor Puppet"),
            };
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Character/Puppet", false, 805)]
        private static void CreateAsset()
        {
            PuppetDialog.ShowCreateDialog();
        }

        [MenuItem("CONTEXT/Puppet/Edit Puppet", false)]
        private static void EditAsset(MenuCommand menuCommand)
        {
            PuppetDialog.ShowEditDialog(menuCommand.context as Puppet);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox(_self.uniqueID, MessageType.None);
            EditorGUILayout.Space();

            if (GUILayout.Button(__content[0]))
            {
                PuppetDialog.ShowEditDialog(_self);
            }

            EditorGUILayout.Space();

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_self);
            }
        }

        private void OnEnable()
        {
            _self = target as Puppet;
        }

        #endregion
    }

}
