// <copyright file="DerivedAttribute.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Character
{

    public sealed class DerivedAttribute : Attribute
    {
        #region Fields
        private readonly AttributeType _type;
        private readonly Actor _actor;        
        #endregion

        #region Properties
        public override int value
        {
            get { return (param + _type.Сalculate(_actor, add, mul)); }
        }
        #endregion

        #region Constructors
        public DerivedAttribute(AttributeType type, Actor actor, int basic, int extra = 0) : base(basic, extra)
        {            
            _type = type;
            _actor = actor;
        }

        private DerivedAttribute() : base()
        {

        }
        #endregion
    }

}
