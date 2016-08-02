// <copyright file="PuppetResists.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Puppets
{

    public enum Resists
    {        
        MagicOfElements,
        MagicOfNature,
        MagicOfDeath,
        Disease,
        Stun,
        Bleed,
        Move,
        None,
    }

    [System.Serializable]
    public class PuppetResists : UniqueObject
    {
        #region Constants
        public const int ResistsCapacity = 32;
        #endregion

        #region Fields
        [SerializeField, HideInInspector]
        private int[] _resists;
        #endregion

        #region Properties

        #endregion

        #region Methods
        public void Initialize()
        {
            _resists = new int[ResistsCapacity];
            _resists[(int)Resists.MagicOfElements] = 0;
            _resists[(int)Resists.MagicOfNature] = 0;
            _resists[(int)Resists.MagicOfDeath] = 0;
            _resists[(int)Resists.Disease] = 0;
            _resists[(int)Resists.Stun] = 0;
            _resists[(int)Resists.Bleed] = 0;
            _resists[(int)Resists.Move] = 0;            
        }

        public int GetResist(Resists resist)
        {
            return _resists[resist.GetIndex()];
        }

        public void SetResist(Resists resist, int value)
        {
            _resists[resist.GetIndex()] = value;
        }
        #endregion
    }

}
