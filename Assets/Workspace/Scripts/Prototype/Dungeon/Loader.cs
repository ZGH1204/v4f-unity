// <copyright file="Loader.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Dungeon
{

    public class Loader : MonoBehaviour
    {
        public Locomotion locomotion;

        public Room[] halls;
        public Room[] corridors;
        public GameObject background;
        public GameObject foreground;

        private void Start()
        {
            //var room = corridors[0];
            var room = corridors[0];
            locomotion.Initialize(room.boundPosition, room.boundCamera);
        }
    }
	
}
