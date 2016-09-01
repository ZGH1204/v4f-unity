// <copyright file="Extension.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Dungeon
{

    public static class Extension
    {
        public static int GetSize(this Size self)
        {
            return (int)self;
        }

        public static Vector2 GetRoomSize(this Size self)
        {
            switch (self)
            {
                case Size.Dungeon_10x10:
                    return new Vector2(32f, 10f);
                default:
                    return Vector2.zero;
            }
        }

        public static Vector2 GetRoomAnchor(this Size self)
        {
            switch (self)
            {
                case Size.Dungeon_10x10:
                    return new Vector2(78f, 1.2f);
                default:
                    return Vector2.zero;
            }
        }

        public static Vector2 GetOffset(this Size self)
        {
            switch (self)
            {
                case Size.Dungeon_10x10:
                    return new Vector2(34f, 34f);
                default:
                    return Vector2.zero;
            }
        }
    }
	
}
