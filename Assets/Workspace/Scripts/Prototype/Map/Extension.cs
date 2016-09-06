// <copyright file="Extension.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Map
{

    public static class Extension
    {
        public static bool Initializer(this Node[] self, int row, out Node[] array)
        {
            if (self == null)
            {                
                var length = row * row;
                array = new Node[length];

                for (var i = 0; i < length; ++i)
                {
                    array[i] = new Node(NodeType.None, i);
                }

                return true;
            }

            array = self;
            return false;
        }        
    }
	
}
