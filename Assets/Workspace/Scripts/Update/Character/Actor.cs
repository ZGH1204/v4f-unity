// <copyright file="Actor.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

using V4F.Game;

namespace V4F.Character
{

    public class Actor
    {
        #region Fields
        private Dictionary<AttributeType, Attribute> _attributes;
        private Puppet _puppet;
        private Location _location;
        #endregion

        #region Properties
        public Attribute this[AttributeType type]
        {
            get { return _attributes[type]; }
        }

        public Puppet puppet
        {
            get { return _puppet; }
        }

        public Location location
        {
            get { return _location; }
            set { _location = value; }
        }
        #endregion

        #region Constructors
        public Actor(Puppet puppet)
        {            
            _puppet = InitializeAttributes(puppet);
        }
        #endregion
        
        #region Methods
        private Puppet InitializeAttributes(Puppet puppet)
        {
            _attributes = new Dictionary<AttributeType, Attribute>(24);

            _attributes.Add(AttributeType.Strength, new Attribute(puppet.spec.strength));
            _attributes.Add(AttributeType.Dexterity, new Attribute(puppet.spec.dexterity));
            _attributes.Add(AttributeType.Magic, new Attribute(puppet.spec.magic));
            _attributes.Add(AttributeType.Vitality, new Attribute(puppet.spec.vitality));
            _attributes.Add(AttributeType.HealthPoints, new DerivedAttribute(AttributeType.HealthPoints, this, puppet.spec.healthPoints));
            _attributes.Add(AttributeType.MinDamageMelee, new DerivedAttribute(AttributeType.MinDamageMelee, this, puppet.spec.minDamageMelee));
            _attributes.Add(AttributeType.MaxDamageMelee, new DerivedAttribute(AttributeType.MaxDamageMelee, this, puppet.spec.maxDamageMelee));
            _attributes.Add(AttributeType.MinDamageRange, new DerivedAttribute(AttributeType.MinDamageRange, this, puppet.spec.minDamageRange));
            _attributes.Add(AttributeType.MaxDamageRange, new DerivedAttribute(AttributeType.MaxDamageRange, this, puppet.spec.maxDamageRange));
            _attributes.Add(AttributeType.MinDamageMagic, new DerivedAttribute(AttributeType.MinDamageMagic, this, puppet.spec.minDamageMagic));
            _attributes.Add(AttributeType.MaxDamageMagic, new DerivedAttribute(AttributeType.MaxDamageMagic, this, puppet.spec.maxDamageMagic));
            _attributes.Add(AttributeType.ChanceToDodge, new Attribute(puppet.spec.chanceToDodge));
            _attributes.Add(AttributeType.ChanceToCrit, new Attribute(puppet.spec.chanceToCrit));
            _attributes.Add(AttributeType.FireResistance, new Attribute(puppet.spec.fireResistance));
            _attributes.Add(AttributeType.IceResistance, new Attribute(puppet.spec.iceResistance));
            _attributes.Add(AttributeType.LightingResistance, new Attribute(puppet.spec.lightingResistance));
            _attributes.Add(AttributeType.DeathResistance, new Attribute(puppet.spec.deathResistance));
            _attributes.Add(AttributeType.PoisonResistance, new Attribute(puppet.spec.poisonResistance));
            _attributes.Add(AttributeType.StunMoveResistance, new Attribute(puppet.spec.stunMoveResistance));
            _attributes.Add(AttributeType.ImmuneResistance, new Attribute(puppet.spec.immuneResistance));
            _attributes.Add(AttributeType.BleedingResistance, new Attribute(puppet.spec.bleedingResistance));

            return puppet;
        }
        #endregion
    }

}
