// <copyright file="MapHandler.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System;

namespace V4F.UI.Map
{

    public class MapEventArgs : EventArgs
    {
	    public int currentRoomIndex { get; set; }
        public int chooseRoomIndex { get; set; }

        public MapEventArgs()
        {
            currentRoomIndex = -1;
            chooseRoomIndex = -1;
        }
    }
	
}
