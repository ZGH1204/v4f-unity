// <copyright file="PuppetStorage.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

namespace V4F.Character
{

    [System.Serializable]
    public class PuppetStorage : ScriptableObject
    {
        #region Constants
        public const int MaxPuppets = 256;
        #endregion

        #region Fields
        [SerializeField, HideInInspector]
        private List<Puppet> _puppets = null;
        #endregion

        #region Properties
        public int countPuppets
        {
            get { return _puppets.Count; }
        }

        public Puppet this[int index]
        {
            get { return _puppets[index]; }
        }
        #endregion

        #region Methods
        public bool TryGetPuppet(string uniqueID, out Puppet puppet)
        {
            puppet = _puppets.Find(x => (string.Compare(x.uniqueID.ToUpper(), uniqueID.ToUpper()) == 0));
            return (puppet != null);
        }

        public bool Contains(string uniqueID)
        {
            var puppet = _puppets.Find(x => (string.Compare(x.uniqueID.ToUpper(), uniqueID.ToUpper()) == 0));
            return (puppet != null);
        }

        private void OnEnable()
        {
            if (_puppets == null)
            {
                _puppets = new List<Puppet>(MaxPuppets);
            }
        }
        #endregion
    }

}
