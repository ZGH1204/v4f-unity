// <copyright file="Link.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

namespace V4F.Prototype.Dungeon
{

    [System.Serializable]
    public class Link
    {
        [SerializeField, HideInInspector]
        private int _subObject1;

        [SerializeField, HideInInspector]
        private int _subObject2;

        [SerializeField, HideInInspector]
        private bool _vertical;

        public int subObject1
        {
            get { return _subObject1; }
        }

        public int subObject2
        {
            get { return _subObject2; }
        }

        public bool vertical
        {
            get { return _vertical; }
        }

        public Link(int subObject1, int subObject2, bool vertical)
        {
            _subObject1 = subObject1;
            _subObject2 = subObject2;
            _vertical = vertical;
        }	

        public class Comparer : IEqualityComparer<Link>
        {
            public bool Equals(Link a, Link b)
            {
                return (a.vertical == b.vertical) &&
                (
                    ((a.subObject1 == b.subObject1) && (a.subObject2 == b.subObject2))
                    ||
                    ((a.subObject1 == b.subObject2) && (a.subObject2 == b.subObject1))
                );
            }

            public int GetHashCode(Link x)
            {
                return x.subObject1.GetHashCode() + x.subObject2.GetHashCode();
            }
        }
    }
	
}
