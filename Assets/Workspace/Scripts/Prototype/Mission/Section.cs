// <copyright file="Section.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Mission
{

    public class Section : MonoBehaviour
    {
        public delegate void SectionEventsHandler(Section sender, SectionEventArgs args);

        public event SectionEventsHandler OnEnter;
        public event SectionEventsHandler OnExit;

        public SectionType type = SectionType.None;

        public Transform[] slots = new Transform[4];

        private void OnEnterCallback(SectionEventArgs args)
        {
            if (OnEnter != null)
            {
                OnEnter(this, args);
            }
        }

        private void OnExitCallback(SectionEventArgs args)
        {
            if (OnExit != null)
            {
                OnExit(this, args);
            }
        }

        private int GetIndex(SectionType type)
        {
            switch (type)
            {
                case SectionType.Enter:
                    return -1;
                case SectionType.Room:
                case SectionType.Span1:
                    return 0;
                case SectionType.Span2:
                    return 1;
                case SectionType.Span3:
                    return 2;
                case SectionType.Span4:
                    return 3;
                case SectionType.Exit:
                    return 4;
                default:
                    return 255;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (type != SectionType.None)
            {
                var index = GetIndex(type);

                var args = new SectionEventArgs(type);                
                args.index = index;
                args.combatCheck = (index == Mathf.Clamp(index, 0, 3));
                OnEnterCallback(args);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (type != SectionType.None)
            {
                var args = new SectionEventArgs(type);
                args.index = GetIndex(type);
                args.combatCheck = false;
                OnExitCallback(args);
            }
        }
    }
	
}
