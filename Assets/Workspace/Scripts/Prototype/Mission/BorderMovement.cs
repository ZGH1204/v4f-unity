// <copyright file="BorderMovement.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Mission
{

    public class BorderMovement : MonoBehaviour
    {
        public Transform leftBorder;
        public Transform rightBorder;
        public Transform leftAnchor;
        public Transform rightAnchor;

        public Vector2 movement
        {
            get
            {
                var l = (leftBorder != null) ? leftBorder.localPosition : Vector3.zero;
                var r = (rightBorder != null) ? rightBorder.localPosition : Vector3.zero;
                return new Vector2(l.x, r.x);
            }
        }

        public Vector2 anchors
        {
            get
            {
                var l = (leftAnchor != null) ? leftAnchor.localPosition : Vector3.zero;
                var r = (rightAnchor != null) ? rightAnchor.localPosition : Vector3.zero;
                return new Vector2(l.x, r.x);
            }
        }
    }
	
}
