// <copyright file="PoolExtension.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F
{

    public static class PoolExtension
    {
        #region Methods
        public static bool Validate(this Poolable self, PoolObjects owner)
        {
            if (self != null)
            {                
                return self.pool.Equals(owner);
            }

            return false;
        }

        public static void ReturnToPool(this GameObject self)
        {
            if (self != null)
            {
                var poolable = self.GetComponent<Poolable>();
                if (poolable != null)
                {
                    var pool = poolable.pool;
                    pool.Push(self);
                }
            }            
        }
        #endregion
    }

}
