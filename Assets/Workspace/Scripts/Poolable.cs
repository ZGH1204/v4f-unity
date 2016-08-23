// <copyright file="Poolable.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F
{

    public class Poolable : MonoBehaviour
    {
        #region Fields
        [HideInInspector]
        private PoolObjects _pool = null;
        #endregion

        #region Properties
        public PoolObjects pool
        {
            get { return _pool; }
            set { _pool = value; }
        }
        #endregion

    }

}
