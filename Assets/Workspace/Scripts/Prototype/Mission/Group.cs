// <copyright file="Group.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

using V4F.Character;

namespace V4F.Prototype.Mission
{

    public abstract class Group : MonoBehaviour
    {
        public delegate void GroupCallback(Group sender);

        public abstract bool isAlive { get; }

        public abstract int count { get; }

        public abstract Actor this[int index] { get; }

        public abstract void PlayTweaking(GroupCallback cb);

        public abstract bool Change(Actor a, Actor b);
    }
	
}
