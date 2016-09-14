// <copyright file="NodePlay.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Map
{

    public class NodeMap
    {
        private NodeType _type = NodeType.None;

        public NodeType type
        {
            get { return _type; }
        }
	
        public NodeMap(NodeType type)
        {
            _type = type;
        }
    }
	
}
