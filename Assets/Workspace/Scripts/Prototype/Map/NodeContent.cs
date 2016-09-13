// <copyright file="NodeContent.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Map
{

    public class NodeContent
    {
        private int[] _materialIndices;

        public static int GetMaterialIndex(int materialIndices, int part)
        {
            var offset = part * 8;
            return (((materialIndices & (0x0000007F << offset)) >> offset) & 0x0000007F);
        }

        public static int SetMaterialIndex(int materialIndices, int part, int value)
        {
            var offset = part * 8;
            return ((materialIndices & ~(0x0000007F << offset)) | ((value & 0x0000007F) << offset));
        }        

        public int materialIndices
        {
            get
            {
                return (_materialIndices[0] & 0x0000007F) |
                    ((_materialIndices[1] <<  8) & 0x00007F00) |
                    ((_materialIndices[2] << 16) & 0x007F0000) |
                    ((_materialIndices[3] << 24) & 0x7F000000);
            }
        }

        public int this[int index]
        {
            get { return _materialIndices[index]; }
        }

        public NodeContent(int materialIndices)
        {
            _materialIndices = new int[]
            {
                GetMaterialIndex(materialIndices, 0),
                GetMaterialIndex(materialIndices, 1),
                GetMaterialIndex(materialIndices, 2),
                GetMaterialIndex(materialIndices, 3),
            };
        }
    }
	
}
