// <copyright file="CreatePuppetEventArgs.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

using V4F.Puppets;

namespace V4F
{

    public class CreatePuppetEventArgs : System.EventArgs
    {
        #region Properties
        public string path { get; set; }
        public PuppetSpec spec { get; set; }
        public PuppetSkillSet skillSet { get; set; }
        public Sprite icon { get; set; }
        public string properName { get; set; }
        public GameObject prefab { get; set; }
        #endregion

        #region Constructors
        public CreatePuppetEventArgs()
        {
            spec = null;
            skillSet = null;
            icon = null;
            properName = null;
            prefab = null;
        }
        #endregion
    }

}
