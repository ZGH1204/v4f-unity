// <copyright file="Dispatcher.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using V4F.Puppets;

namespace V4F
{

    [InitializeOnLoad]
    public sealed class Dispatcher
    {
        #region Fields        
        private static readonly Dispatcher __instance = null;        
        #endregion

        #region Properties        
        public static Dispatcher singleton
        {
            get { return __instance; }
        }
        #endregion

        #region Constructors        
        static Dispatcher()
        {
            __instance = new Dispatcher();
        }

        private Dispatcher()
        {
            Debug.Log("Default constructor for Dispatcher");
        }
        #endregion

        #region Methods        
        [DidReloadScripts]
        private static void OnReloadScript()
        {
            Debug.Log("Reload method for Dispatcher");
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            PuppetDialog.OnCreate += OnCreatePuppetResultCallback;
            PuppetDialog.OnEdit += OnEditPuppetResultCallback;
        }

        private static void OnEditPuppetResultCallback(PuppetDialog sender, PuppetEventArgs args)
        {
            __instance.OnEditPuppetResult(sender, args);
        }

        private static void OnCreatePuppetResultCallback(PuppetDialog sender, PuppetEventArgs args)
        {
            __instance.OnCreatePuppetResult(sender, args);
        }        

        private void OnEditPuppetResult(PuppetDialog sender, PuppetEventArgs args)
        {
            var puppet = args.puppet;
            if (puppet != null)
            {
                var serializedObject = new SerializedObject(puppet);
                serializedObject.Update();

                var spec = serializedObject.FindProperty("_spec");
                spec.objectReferenceValue = args.spec;
                
                var skillSet = serializedObject.FindProperty("_skillSet");
                skillSet.objectReferenceValue = args.skillSet;
                
                var properName = serializedObject.FindProperty("_name");                
                properName.stringValue = args.properName;

                var prefab = serializedObject.FindProperty("_prefab");
                prefab.objectReferenceValue = args.prefab;

                var icon = serializedObject.FindProperty("_icon");
                icon.objectReferenceValue = args.icon;

                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnCreatePuppetResult(PuppetDialog sender, PuppetEventArgs args)
        {
            var puppet = ScriptableHelper.CreateAsset<Puppet>(args.path);
            if (puppet != null)
            {
                var serializedObject = new SerializedObject(puppet);
                serializedObject.Update();

                var spec = serializedObject.FindProperty("_spec");
                spec.objectReferenceValue = args.spec;

                var skillSet = serializedObject.FindProperty("_skillSet");
                skillSet.objectReferenceValue = args.skillSet;

                var properName = serializedObject.FindProperty("_name");
                properName.stringValue = args.properName;

                var prefab = serializedObject.FindProperty("_prefab");
                prefab.objectReferenceValue = args.prefab;

                var icon = serializedObject.FindProperty("_icon");
                icon.objectReferenceValue = args.icon;

                serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion
    }

}
