// <copyright file="SectionEventArgs.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System;

namespace V4F.Prototype.Dungeon
{

    public class SectionEventArgs : EventArgs
    {
        public SectionType type { get; set; }
        public int id { get; set; }
    }
	
}
