// <copyright file="AttributeModifier.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Character
{

    public class AttributeModifier : ScriptableObject
    {
        #region Fields
        private int _add = 0;
        private float _mul = 0f;
        #endregion

        #region Properties
        public int add
        {
            get { return _add; }
        }

        public float mul
        {
            get { return _mul; }
        }
        #endregion

        #region Methods
        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }
        #endregion
    }

}
