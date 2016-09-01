// <copyright file="Preset.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Dungeon
{

    [System.Serializable]
    public class Preset : ScriptableObject
    {
        #region Fields        
        [HideInInspector]
        public int[] halls = null;

        [HideInInspector]
        public Link[] links = null;
        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion
    }

}
