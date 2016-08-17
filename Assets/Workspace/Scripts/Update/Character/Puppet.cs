// <copyright file="Puppet.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Character
{

    [System.Serializable]
    public class Puppet : UniqueObject
    {
        #region Fields
        [SerializeField, HideInInspector]
        private Specification _spec;

        [SerializeField, HideInInspector]
        private SkillSet _skillSet;

        [SerializeField, HideInInspector]
        private Sprite _icon;

        [SerializeField, HideInInspector]
        private Classes _charClass;

        [SerializeField, HideInInspector]
        private string _properName;

        [SerializeField, HideInInspector]
        private GameObject _prefab;
        #endregion

        #region Properties
        public Specification spec
        {
            get { return _spec; }
        }

        public SkillSet skillSet
        {
            get { return _skillSet; }
        }

        public Sprite icon
        {
            get { return _icon; }
        }

        public Classes charClass
        {
            get { return _charClass; }
        }

        public string properName
        {
            get { return _properName; }
        }

        public GameObject prefab
        {
            get { return _prefab; }
        }
        #endregion
    }

}
