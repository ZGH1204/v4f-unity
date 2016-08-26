// <copyright file="CreatePuppetEventArgs.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

using V4F.Character;

namespace V4F
{

    public class PuppetEventArgs : System.EventArgs
    {
        #region Properties    
        public string path { get; set; }
        public Puppet puppet { get; set; }
        public Specification spec { get; set; }
        public SkillSet skillSet { get; set; }
        public Sprite icon { get; set; }
        public Classes charClass { get; set; }
        public string properName { get; set; }
        public GameObject prefab { get; set; }
        public GameObject prefabUI { get; set; }
        #endregion

        #region Constructors
        public PuppetEventArgs()
        {
            spec = null;
            skillSet = null;
            icon = null;
            charClass = Classes.Warrior;
            properName = "#BAD_NAME";
            prefab = null;
            prefabUI = null;
        }
        #endregion
    }

}
