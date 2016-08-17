// <copyright file="Puppet.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Character
{

    [System.Serializable]
    public class Puppet : UniqueObject
    {
        #region Fields
        [SerializeField, HideInInspector]
        private Specification _spec;
        #endregion

        #region Properties
        public Specification spec
        {
            get { return _spec; }
        }
        #endregion
    }

}
