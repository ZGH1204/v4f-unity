// <copyright file="Effect.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Character
{

    [System.Serializable]
    public class Effect : UniqueObject
    {
        #region Fields
        [SerializeField, HideInInspector]
        private string _title = null;

        [SerializeField, HideInInspector]
        private string _description = null;

        [SerializeField, HideInInspector]
        private ResistanceType _resistType = ResistanceType.None;

        [SerializeField, HideInInspector]
        private int _timer = 3;

        [SerializeField, HideInInspector]
        private int _minDamage = 1;

        [SerializeField, HideInInspector]
        private int _maxDamage = 2;

        [SerializeField, HideInInspector]
        private bool _reverseDamage = false;

        [SerializeField, HideInInspector]
        private bool _makeDamage = true;
        #endregion

        #region Properties
        public string title
        {
            get { return _title; }
        }

        public string description
        {
            get { return _description; }
        }

        public ResistanceType resistType
        {
            get { return _resistType; }
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

        public bool reverseDamage
        {
            get { return _reverseDamage; }
        }

        public bool makeDamage
        {
            get { return _makeDamage; }
        }
        #endregion

        #region Methods
        protected override void OnEnable()
        {
            base.OnEnable();

            if (string.IsNullOrEmpty(_title))
            {
                _title = "#BAD_TITLE";
            }

            if (string.IsNullOrEmpty(_description))
            {
                _description = "#BAD_DESCRIPTION";
            }
        }
        #endregion
    }

}
