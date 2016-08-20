// <copyright file="UniqueObject.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F
{

    [System.Serializable]
    public class UniqueObject : ScriptableObject
    {
        #region Fields
        [SerializeField, HideInInspector]
        private string _uniqueID = null;
        #endregion

        #region Properties
        public string uniqueID
        {
            get { return _uniqueID; }
        }
        #endregion

        #region Methods
        protected virtual void OnEnable()
        {
            if (string.IsNullOrEmpty(_uniqueID))
            {
                _uniqueID = System.Guid.NewGuid().ToString();
            }
        }
        #endregion
    }

}
