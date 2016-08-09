// <copyright file="DraggableEventArgs.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System;

using UnityEngine;

namespace V4F.UI
{

    public class DraggableEventArgs : EventArgs
    {
        #region Properties
        public DraggableType type { get; set; }
        public Vector2 point { get; set; }
        public Camera camera { get; set; }
        #endregion
    }

}
