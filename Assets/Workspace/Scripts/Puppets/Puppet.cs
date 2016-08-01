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
        private string _name = "#BAD_NAME";

        [SerializeField, HideInInspector]
        private Sprite _icon = null;

        [SerializeField, HideInInspector]
        private GameObject _prefab = null;

        [SerializeField, HideInInspector]
        private PuppetClass _class = PuppetClass.Warrior;
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

        public Sprite icon
        {
            get { return _icon; }
        }

        public GameObject prefab
        {
            get { return _prefab; }
        }

        public PuppetClass puppetClass
        {
            get { return _class; }
        }
        #endregion
    }

}
