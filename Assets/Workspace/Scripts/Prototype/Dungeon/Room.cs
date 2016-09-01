// <copyright file="Room.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Dungeon
{

    public abstract class Room : MonoBehaviour
    {
        public abstract Vector3 boundPosition { get; }
        public abstract Vector3 boundCamera { get; }
    }

}
