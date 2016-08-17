// <copyright file="SkillStage.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

namespace V4F.Character
{

    [System.Serializable]
    public class SkillStage : ScriptableObject
    {
        #region Constants
        public const int MaxEffects = 32;
        #endregion

        #region Fields
        [SerializeField, HideInInspector]
        private List<Effect> _effects = new List<Effect>(MaxEffects);

        [SerializeField, HideInInspector]
        private List<bool> _effectsPassed = new List<bool>(MaxEffects);

        [SerializeField, HideInInspector]
        private Sprite _icon = null;

        [SerializeField, HideInInspector]
        private string _title = "#BAD_TITLE";

        [SerializeField, HideInInspector]
        private string _description = "#BAD_DESCRIPTION";

        [SerializeField, HideInInspector]
        private DamageType _damageType = DamageType.None;

        [SerializeField, HideInInspector]
        private ResistanceType _resistType = ResistanceType.None;

        [SerializeField, HideInInspector]
        private SkillGoal _goal = SkillGoal.None;

        [SerializeField, HideInInspector]
        private int _position = 0x000000F1;

        [SerializeField, HideInInspector]
        private bool _splash = false;

        [SerializeField, HideInInspector]
        private int _move = 0;

        [SerializeField, HideInInspector]
        private int _moveСhance = 0;

        [SerializeField, HideInInspector]
        private bool _stun = false;

        [SerializeField, HideInInspector]
        private int _stunСhance = 0;

        [SerializeField, HideInInspector]
        private int _damageModifier = 0;
        #endregion

        #region Properties
        public Sprite icon
        {
            get { return _icon; }
        }

        public string title
        {
            get { return _title; }
        }

        public string description
        {
            get { return _description; }
        }

        public DamageType damageType
        {
            get { return _damageType; }
        }

        public ResistanceType resistType
        {
            get { return _resistType; }
        }

        public SkillGoal goal
        {
            get { return _goal; }
        }

        public int position
        {
            get { return _position; }
        }

        public bool splash
        {
            get { return _splash; }
        }

        public int move
        {
            get { return _move; }
        }

        public int moveChance
        {
            get { return _moveСhance; }
        }

        public bool stun
        {
            get { return _stun; }
        }

        public int stunChance
        {
            get { return _stunСhance; }
        }

        public int damageModifier
        {
            get { return _damageModifier; }
        }

        public int countEffects
        {
            get { return _effects.Count; }
        }

        public Effect this[int index]
        {
            get { return (_effectsPassed[index] ? _effects[index] : null); }
        }
        #endregion

        #region Methods
        public bool validate
        {
            get
            {
                var valid = _icon != null;
                valid = valid && (_goal != SkillGoal.None);
                //valid = valid && string.IsNullOrEmpty(_title);
                //valid = valid && string.IsNullOrEmpty(_description);
                //valid = valid && (_damageType != DamageType.None);
                //valid = valid && (_resistType != ResistanceType.None);                
                return valid;
            }
        }
        #endregion
    }

}
