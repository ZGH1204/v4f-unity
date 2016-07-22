// <copyright file="PuppetSpec.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Puppets
{

    [System.Serializable]
    public class PuppetSpec : ScriptableObject
    {
        #region Fields
        [SerializeField, HideInInspector]
        private int _healthPoints = 1;

        [SerializeField, HideInInspector]
        private int _accuracy = 1;

        [SerializeField, HideInInspector]
        private int _initiative = 1;
        #endregion

        #region Properties
        public int healthPoints
        {
            get { return _healthPoints; }
            set { _healthPoints = value; }
        }

        public int accuracy
        {
            get { return _accuracy; }
            set { _accuracy = value; }
        }

        public int initiative
        {
            get { return _initiative; }
            set { _initiative = value; }
        }
        #endregion
    }

}
