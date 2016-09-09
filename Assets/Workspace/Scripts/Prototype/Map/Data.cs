// <copyright file="Data.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Map
{

    [System.Serializable]
    public class Data : ScriptableObject
    {
        #region Constants
        public const int RoomsCount = 19;
        public const int RowCount = RoomsCount + (RoomsCount - 1);
        #endregion

        #region Fields
        [SerializeField, HideInInspector]
        private Node[] _nodes;

        [SerializeField, HideInInspector]
        private int _length;

        [SerializeField, HideInInspector]
        private int _width;

        [SerializeField, HideInInspector]
        private int _entry;
        #endregion

        #region Properties
        public Node this[int index]
        {
            get { return _nodes[index]; }
            set { _nodes[index] = value; }
        }

        public int length
        {
            get { return _length; }
        }

        public int width
        {
            get { return _width; }
        }

        public int entry
        {
            get { return _entry; }
        }
        #endregion

        #region Methods
        public Data Initializer()
        {
            _length = RowCount * RowCount;
            _nodes = new Node[length];
            for (var i = 0; i < length; ++i)
            {
                _nodes[i] = new Node(NodeType.None, i);
            }            

            _entry = (RowCount + 1) * (RowCount / 2);
            _nodes[_entry] = new Node(NodeType.Room, _entry);

            _width = RowCount;

            return this;
        }        
        #endregion
    }

}
