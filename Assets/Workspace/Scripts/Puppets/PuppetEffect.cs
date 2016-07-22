// <copyright file="PuppetEffect.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Puppets
{

    [System.Serializable]
    public class PuppetEffect : ScriptableObject
    {
        #region Fields
        [SerializeField, HideInInspector]
        private string _title = "New effect";

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
            set { _title = value; }
        }

        public int timer
        {
            get { return _timer; }
            set { _timer = Mathf.Max(0, value); }
        }

        public int minDamage
        {
            get { return _minDamage; }
            set { _minDamage = Mathf.Min(_maxDamage, value); }
        }

        public int maxDamage
        {
            get { return _maxDamage; }
            set { _maxDamage = Mathf.Max(_minDamage, value); }
        }

        public bool invertDamage
        {
            get { return _invertDamage; }
            set { _invertDamage = value; }
        }

        public bool applyDamage
        {
            get { return _applyDamage; }
            set { _applyDamage = value; }
        }

        public bool stun
        {
            get { return _stun; }
            set { _stun = value; }
        }

        public bool finished
        {
            get { return (_timer < 0); }
        }
        #endregion

        #region Constructors

        #endregion

        #region Methods
        
        #endregion
    }

}
