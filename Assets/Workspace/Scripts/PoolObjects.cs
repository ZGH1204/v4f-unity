// <copyright file="PoolObjects.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

namespace V4F
{

    public class PoolObjects : MonoBehaviour
    {
        #region Fields        
        public GameObject prefab = null;

        private List<GameObject> _pool;
        #endregion

        #region Methods        
        public override bool Equals(object obj)
        {
            if (!(obj is PoolObjects))
            {
                return false;
            }

            return Equals((PoolObjects)obj);
        }

        public bool Equals(PoolObjects pool)
        {            
            if ((object)pool == null)
            {
                return false;
            }                

            return (GetInstanceID() == pool.GetInstanceID());
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public GameObject Pop()
        {
            GameObject go = null;
            Poolable poolable = null;

            if (_pool.Count > 0)
            {
                go = _pool[0];

                poolable = go.GetComponent<Poolable>();

                _pool.RemoveAt(0);
            }
            else
            {
                go = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;

                poolable = go.AddComponent<Poolable>();
            }

            poolable.pool = this;

            return go;
        }

        public bool Push(GameObject go)
        {
            var poolable = go.GetComponent<Poolable>();
            if (poolable.Validate(this))
            {
                poolable.pool = null;

                go.SetActive(false);
                go.transform.SetParent(transform);
                go.transform.SetAsLastSibling();

                _pool.Add(go);

                return true;
            }

            return false;
        }

        private void Awake()
        {
            _pool = new List<GameObject>(32);

            if (prefab.activeSelf)
            {
                Debug.LogErrorFormat("Prefab \"{0}\" should be not active!", prefab.name);
                return;
            }
        }
        #endregion

    }

}
