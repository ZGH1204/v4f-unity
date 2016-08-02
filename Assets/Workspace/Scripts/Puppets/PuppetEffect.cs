// <copyright file="PuppetEffect.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Puppets
{

    [System.Serializable]
    public class PuppetEffect : UniqueObject
    {
        #region Fields
        [SerializeField, HideInInspector]
        private string _title = "#BAD_TITLE";

        [SerializeField, HideInInspector]
        private Resists _resist = Resists.None;

        [SerializeField, HideInInspector]
        private int _timer = 3;

        [SerializeField, HideInInspector]
        private int _minDamage = 1;

        [SerializeField, HideInInspector]
        private int _maxDamage = 2;

        [SerializeField, HideInInspector]
        private bool _invertDamage = false;

        [SerializeField, HideInInspector]
        private bool _applyDamage = true;

        [SerializeField, HideInInspector]
        private bool _stun = false;
        #endregion

        #region Properties
        public string title
        {
            get { return _title; }            
        }

        public Resists resist
        {
            get { return _resist; }
        }

        public int timer
        {
            get { return _timer; }            
        }

        public int minDamage
        {
            get { return _minDamage; }            
        }

        public int maxDamage
        {
            get { return _maxDamage; }            
        }

        public bool invertDamage
        {
            get { return _invertDamage; }            
        }

        public bool applyDamage
        {
            get { return _applyDamage; }            
        }

        public bool stun
        {
            get { return _stun; }
        }
        #endregion

        #region Constructors

        #endregion

        #region Methods
        
        #endregion
    }

}
