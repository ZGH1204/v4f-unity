// <copyright file="Section.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Dungeon
{

    public class Section : MonoBehaviour
    {
        public delegate void SectionEventHandler(Section sender, SectionEventArgs args);

        public static event SectionEventHandler OnEntry;

        public SectionType type;

        public int id;        

        private static void OnEntryCallback(Section sender, SectionEventArgs args)
        {
            if (OnEntry != null)
            {
                OnEntry(sender, args);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var args = new SectionEventArgs();
            args.type = type;
            args.id = id;
            OnEntryCallback(this, args);
        }

    }
	
}
