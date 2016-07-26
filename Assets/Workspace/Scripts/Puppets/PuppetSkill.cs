// <copyright file="PuppetSkill.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

namespace V4F.Puppets
{

    [System.Serializable]
    public class PuppetSkill : ScriptableObject
    {
        #region Constants
        public const int MaxEffects = 32;
        #endregion

        #region Fields
        [SerializeField, HideInInspector]
        private Sprite _icon = null;

        [SerializeField, HideInInspector]
        private string _title = "New skill";

        [SerializeField, HideInInspector]
        private PuppetSkillTarget _useTo = PuppetSkillTarget.Enemies;

        [SerializeField, HideInInspector]
        private bool _combine = false;
        
        [SerializeField, HideInInspector]
        private int _position = 0x000000F1;

        [SerializeField, HideInInspector]
        private int _move = 0;

        [SerializeField, HideInInspector]
        private float _damageModifier = 0f;

        [SerializeField, HideInInspector]
        private float _accuracyModifier = 0f;

        [SerializeField, HideInInspector]
        private float _critChanceModifier = 0f;

        [SerializeField, HideInInspector]
        private float _critDamageModifier = 0f;

        [SerializeField, HideInInspector]
        private List<PuppetEffect> _effects = new List<PuppetEffect>(MaxEffects);

        [SerializeField, HideInInspector]
        private List<bool> _effectsPassed = new List<bool>(MaxEffects);
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

        public PuppetSkillTarget useTo
        {
            get { return _useTo; }            
        }

        public bool combine
        {
            get { return _combine; }            
        }

        public bool isMoved
        {
            get { return (_move != 0); }
        }

        public int move
        {
            get { return _move; }
        }

        public float damageModifier
        {
            get { return _damageModifier; }            
        }

        public float accuracyModifier
        {
            get { return _accuracyModifier; }            
        }

        public float critChanceModifier
        {
            get { return _critChanceModifier; }            
        }

        public float critDamageModifier
        {
            get { return _critDamageModifier; }            
        }

        public int countEffects
        {
            get { return _effects.Count; }
        }

        public PuppetEffect this[int index]
        {
            get { return (_effectsPassed[index] ? _effects[index] : null); }
        }
        #endregion

        #region Methods
        public bool IsPositionForLaunch(int positionNumber)
        {
            var number = positionNumber - 1;
            if (number == Mathf.Clamp(number, 0, 3))
            {
                return ((_position & (1 << number)) != 0);
            }

            return false;
        }        

        public bool IsPositionForTarget(int positionNumber)
        {
            var number = positionNumber - 1;
            if (number == Mathf.Clamp(number, 0, 3))
            {
                return ((_position & ((1 << number) << 4)) != 0);
            }

            return false;
        }
        #endregion
    }

}
