// <copyright file="Specification.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Character
{

    [System.Serializable]
    public class Specification : ScriptableObject
    {
        #region Fields
        [SerializeField, HideInInspector]
        private int[] _attributes = null;

        [SerializeField, HideInInspector]
        private float[] _factors = null;
        #endregion

        #region Properties
        public int strength
        {
            get { return _attributes[0]; }
        }

        public int STR
        {
            get { return strength; }
        }

        public int dexterity
        {
            get { return _attributes[1]; }
        }

        public int DEX
        {
            get { return dexterity; }
        }

        public int magic
        {
            get { return _attributes[2]; }
        }

        public int MAG
        {
            get { return magic; }
        }

        public int vitality
        {
            get { return _attributes[3]; }
        }

        public int VIT
        {
            get { return vitality; }
        }

        public int healthPoints
        {
            get { return _attributes[4]; }
        }

        public int HP
        {
            get { return healthPoints; }
        }

        public float healthPointsFactor
        {
            get { return _factors[4]; }
        }

        public int minDamageMelee
        {
            get { return _attributes[5]; }
        }

        public int MINDMGMELEE
        {
            get { return minDamageMelee; }
        }

        public float minDamageMeleeFactor
        {
            get { return _factors[5]; }
        }

        public int maxDamageMelee
        {
            get { return _attributes[6]; }
        }

        public int MAXDMGMELEE
        {
            get { return maxDamageMelee; }
        }

        public float maxDamageMeleeFactor
        {
            get { return _factors[6]; }
        }

        public int minDamageRange
        {
            get { return _attributes[7]; }
        }

        public int MINDMGRANGE
        {
            get { return minDamageRange; }
        }

        public float minDamageRangeFactor
        {
            get { return _factors[7]; }
        }

        public int maxDamageRange
        {
            get { return _attributes[8]; }
        }

        public int MAXDMGRANGE
        {
            get { return maxDamageRange; }
        }

        public float maxDamageRangeFactor
        {
            get { return _factors[8]; }
        }

        public int minDamageMagic
        {
            get { return _attributes[9]; }
        }

        public int MINDMGMAGIC
        {
            get { return minDamageMagic; }
        }

        public float minDamageMagicFactor
        {
            get { return _factors[9]; }
        }

        public int maxDamageMagic
        {
            get { return _attributes[10]; }
        }

        public int MAXDMGMAGIC
        {
            get { return maxDamageMagic; }
        }

        public float maxDamageMagicFactor
        {
            get { return _factors[10]; }
        }

        public int chanceToDodge
        {
            get { return _attributes[11]; }
        }

        public float chanceToDodgeFactor
        {
            get { return _factors[11]; }
        }

        public int chanceToCrit
        {
            get { return _attributes[12]; }
        }

        public float chanceToCritFactor
        {
            get { return _factors[12]; }
        }

        public int fireResistance
        {
            get { return _attributes[13]; }
        }

        public int iceResistance
        {
            get { return _attributes[14]; }
        }

        public int lightingResistance
        {
            get { return _attributes[15]; }
        }

        public int deathResistance
        {
            get { return _attributes[16]; }
        }

        public int poisonResistance
        {
            get { return _attributes[17]; }
        }

        public int stunMoveResistance
        {
            get { return _attributes[18]; }
        }

        public int immuneResistance
        {
            get { return _attributes[19]; }
        }

        public int bleedingResistance
        {
            get { return _attributes[20]; }
        }
        #endregion

        #region Methods
        public void Initialize()
        {
            _attributes = new int[32] { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            _factors = new float[32] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
        }

        private void Reset()
        {
            Initialize();
        }
        #endregion
    }

}
