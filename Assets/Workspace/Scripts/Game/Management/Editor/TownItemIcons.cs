// <copyright file="TownItemIcons.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace V4F.Game.Management
{

    [InitializeOnLoad]
    public class TownItemIcons
    {
        #region Fields
        private static readonly Texture     __unlocked;
        private static readonly Texture     __locked;
        private static readonly List<int> __itemsUnlocked;
        private static readonly List<int> __itemsLocked;
        #endregion

        #region Constructors
        static TownItemIcons()
        {
            __unlocked = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/item_passed_on.png");
            __locked = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/item_passed_off.png");

            __itemsUnlocked = new List<int>(16);
            __itemsLocked = new List<int>(16);

            EditorApplication.update += Update;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }
        #endregion

        #region Methods
        private static void Update()
        {
            __itemsUnlocked.Clear();
            __itemsLocked.Clear();

            var objects = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];            

            foreach (var obj in objects)
            {
                var townItem = obj.GetComponent<TownItem>();
                if (townItem != null)
                {
                    if (townItem.unlocked)
                    {
                        __itemsUnlocked.Add(obj.GetInstanceID());
                    }
                    else
                    {
                        __itemsLocked.Add(obj.GetInstanceID());                        
                    }                    
                }                    
            }
        }

        private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            Rect r = new Rect(selectionRect);
            r.x = r.width - 20;
            r.width = 18;

            if (__itemsUnlocked.Contains(instanceID))
            {
                GUI.Label(r, __unlocked);
            }
            else if (__itemsLocked.Contains(instanceID))
            {
                GUI.Label(r, __locked);
            }
        }
        #endregion
    }

}
