// <copyright file="Puppet.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Puppets
{

    [System.Serializable]
    public class Puppet : UniqueObject
    {
        #region Fields        
        [SerializeField, HideInInspector]
        private PuppetSpec _spec = null;

        [SerializeField, HideInInspector]
        private PuppetSkillSet _skillSet = null;

        [SerializeField, HideInInspector]
        private string _name = "Puppet";

        [SerializeField, HideInInspector]
        private Sprite _smallIcon = null;
        #endregion

        #region Properties
        public PuppetSpec spec
        {
            get { return _spec; }
        }

        public PuppetSkillSet skillSet
        {
            get { return _skillSet; }
        }

        public string properName
        {
            get { return _name; }
        }

        public Sprite smallIcon
        {
            get { return _smallIcon; }
        }
        #endregion
    }

}
