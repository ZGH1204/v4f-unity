// <copyright file="Attribute.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

namespace V4F.Character
{

    public class Attribute
    {
        #region Fields
        private readonly int _basic;
        private int _extra;
        private int _m_add;
        private float _m_mul;
        private AttributeModifier[] _modifiers = null;        
        private List<int> _freeIndices = null;
        #endregion

        #region Properties
        public virtual int value
        {
            get { return Mathf.FloorToInt(((_basic + _extra) + add) * (1f + mul)); }
        }

        public int basic
        {
            get { return _basic; }
        }

        public int extra
        {
            get { return _extra; }
        }

        public int param
        {
            get { return _basic + _extra; }
        }        

        protected int add
        {
            get { return _m_add; }
        }

        protected float mul
        {
            get { return Mathf.Clamp(_m_mul, 0f, 10f); }
        }
        #endregion

        #region Constructors
        public Attribute(int basic, int extra = 0)
        {
            _basic = basic;
            _extra = extra;
            _m_add = 0;
            _m_mul = 0f;

            _modifiers = new AttributeModifier[16];
            _freeIndices = new List<int>(16);            
            for (var i = 0; i < _modifiers.Length; ++i)
            {
                _freeIndices.Add(i);
            }
        }

        protected Attribute()
        {
            
        }
        #endregion

        #region Methods
        public void Upgrade(int up)
        {
            _extra += up;
        }

        public int AddModifier(AttributeModifier modifier)
        {
            var index = _freeIndices[0];
            _freeIndices.RemoveAt(0);

            _modifiers[index] = modifier;
            _m_add += modifier.add;
            _m_mul += modifier.mul;

            return index;
        }

        public void RemoveModifier(int id)
        {
            var modifier = _modifiers[id];
            _m_add -= modifier.add;
            _m_mul -= modifier.mul;

            _modifiers[id] = null;
            _freeIndices.Add(id);
        }
        #endregion
    }

}
