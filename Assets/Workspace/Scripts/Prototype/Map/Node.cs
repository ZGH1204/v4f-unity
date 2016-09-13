// <copyright file="Node.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Map
{

    [System.Serializable]
    public class Node
    {        
        #region Fields
        [SerializeField]
        private NodeType _type;

        [SerializeField]
        private int _index;
        #endregion

        #region Properties
        public NodeType type
        {
            get { return _type; }
        }

        public int index
        {
            get { return _index; }
        }
        #endregion

        #region Constructors
        public Node(NodeType type, int index)
        {
            _type = type;
            _index = index;
        }
        #endregion

        #region Methods        
        #endregion
    }

}
