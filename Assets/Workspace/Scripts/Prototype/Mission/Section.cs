// <copyright file="Section.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

using V4F.Game;
using V4F.Character;

namespace V4F.Prototype.Mission
{

    public class Section : MonoBehaviour
    {
        public delegate void SectionEventsHandler(Section sender, SectionEventArgs args);

        public event SectionEventsHandler OnEnter;
        public event SectionEventsHandler OnExit;

        public SectionType type = SectionType.None;

        public Squad squad;

        public void Spawn(int count)
        {
            var enemies = Database.QueryRandomEnemies(count);

            squad.Prepare(count);
            for (var i = 0; (i < count) && squad.Spawn(enemies[i]); ++i);
        }

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

        private int GetSubPosition(SectionType type)
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
                var sub = GetSubPosition(type);

                var args = new SectionEventArgs(type);                
                args.subPosition = sub;
                args.combatCheck = (sub == Mathf.Clamp(sub, 0, 3));
                OnEnterCallback(args);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (type != SectionType.None)
            {
                var args = new SectionEventArgs(type);
                args.subPosition = GetSubPosition(type);
                args.combatCheck = false;
                OnExitCallback(args);
            }
        }
    }
	
}
