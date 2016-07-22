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
        private PuppetSkillTarget _useOnTarget = PuppetSkillTarget.Enemies;

        [SerializeField, HideInInspector]
        private int _targetsQuantity = 1;
        
        [SerializeField, HideInInspector]
        private int _targetPosition = 0x000000F1;

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
            set { _icon = value; }
        }

        public string title
        {
            get { return _title; }
            set { _title = value; }
        }

        public PuppetSkillTarget useOnTarget
        {
            get { return _useOnTarget; }
            set { _useOnTarget = value; }
        }

        public int targetsQuantity
        {
            get { return _targetsQuantity; }
            set { _targetsQuantity = value; }
        }

        public float damageModifier
        {
            get { return _damageModifier; }
            set { _damageModifier = Mathf.Clamp01(value); }
        }

        public float accuracyModifier
        {
            get { return _accuracyModifier; }
            set { _accuracyModifier = Mathf.Clamp01(value); }
        }

        public float critChanceModifier
        {
            get { return _critChanceModifier; }
            set { _critChanceModifier = Mathf.Clamp01(value); }
        }

        public float critDamageModifier
        {
            get { return _critDamageModifier; }
            set { _critDamageModifier = Mathf.Max(0f, value); }
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
        public bool CanUseInPosition(int positionNumber)
        {
            var number = positionNumber - 1;
            if (number == Mathf.Clamp(number, 0, 3))
            {
                return ((_targetPosition & (1 << number)) != 0);
            }

            return false;
        }

        public void UseInPosition(int positionNumber, bool yesNo)
        {
            var number = positionNumber - 1;
            if (number == Mathf.Clamp(number, 0, 3))
            {
                var flag = (1 << number);
                if (yesNo)
                {
                    _targetPosition |= flag;
                }
                else
                {
                    _targetPosition &= ~flag;
                }
            }
        }


        public bool CanUseToTargetPosition(int positionNumber)
        {
            var number = positionNumber - 1;
            if (number == Mathf.Clamp(number, 0, 3))
            {
                return ((_targetPosition & ((1 << number) << 4)) != 0);
            }

            return false;
        }

        public void UseToTargetPosition(int positionNumber, bool yesNo)
        {
            var number = positionNumber - 1;
            if (number == Mathf.Clamp(number, 0, 3))
            {
                var flag = ((1 << number) << 4);
                if (yesNo)
                {
                    _targetPosition |= flag;
                }
                else
                {
                    _targetPosition &= ~flag;
                }
            }
        }        
        #endregion
    }

}
