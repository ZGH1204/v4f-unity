// <copyright file="SectionEventArgs.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System;

namespace V4F.Prototype.Mission
{

    public class SectionEventArgs : EventArgs
    {
        public SectionType type { get; set; }
        public int subPosition { get; set; }
        public bool combatCheck { get; set; }
	
        public SectionEventArgs(SectionType type)
        {
            this.type = type;
        }
    }
	
}
