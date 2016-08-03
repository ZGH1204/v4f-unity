// <copyright file="TouchEventArgs.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F
{

    public class TouchEventArgs : System.EventArgs
    {
        #region Properties
        public int fingerId { get; set; }
        public Vector3 position { get; set; }
        public Vector3 offset { get; set; }
        #endregion

        #region Constructors
        public TouchEventArgs()
        {
            fingerId = -1;
            position = Vector3.zero;
            offset = Vector3.zero;
        }

        public TouchEventArgs(int fingerId)
        {
            this.fingerId = fingerId;
            position = Vector3.zero;
            offset = Vector3.zero;
        }

        public TouchEventArgs(int fingerId, Vector3 position)
        {
            this.fingerId = fingerId;
            this.position = position;
            offset = Vector3.zero;
        }

        public TouchEventArgs(int fingerId, float x, float y)
        {
            this.fingerId = fingerId;
            position = new Vector3(x, y);
            offset = Vector3.zero;
        }

        public TouchEventArgs(int fingerId, Vector3 position, Vector3 offset)
        {
            this.fingerId = fingerId;
            this.position = position;
            this.offset = offset;
        }

        public TouchEventArgs(int fingerId, float x, float y, float dx, float dy)
        {
            this.fingerId = fingerId;
            position = new Vector3(x, y);
            offset = new Vector3(dx, dy);
        }
        #endregion
    }

}
