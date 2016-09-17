// <copyright file="NodePlay.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Map
{

    public class NodeMap
    {
        private NodeType _type = NodeType.None;
        private int[] _combat = new int[4];

        public NodeType type
        {
            get { return _type; }
        }
	
        public NodeMap(NodeType type)
        {
            _type = type;
        }

        public void SetCombat(int index, bool combat)
        {
            if (_type == NodeType.Room)
            {
                index = 0;
            }

            if (combat && (_combat[index] == -1))
            {
                _combat[index] = 0;
            }
            else if (!combat && (_combat[index] == 0))
            {
                _combat[index] = 1;
            }
        }

        public bool IsCombat(int index)
        {
            if (_type == NodeType.Room)
            {
                index = 0;
            }

            return (_combat[index] == 0);
        }

        public bool WasCombat(int index)
        {
            if (_type == NodeType.Room)
            {
                index = 0;
            }

            return (_combat[index] == 1);
        }
    }
	
}
