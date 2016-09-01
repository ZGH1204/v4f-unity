// <copyright file="Corridor.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Dungeon
{

    public class Corridor : Room
    {
        public Transform leftBoundPosition;
        public Transform rightBoundPosition;
        public Transform leftBoundCamera;
        public Transform rightBoundCamera;
        public bool leftStartPosition;

        public override Vector3 boundPosition
        {
            get
            {
                var ret = Vector3.zero;

                if (leftBoundPosition != null)
                {
                    ret.x = leftBoundPosition.position.x;
                    if (leftStartPosition)
                    {
                        ret.z = ret.x;
                    }
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogErrorFormat("For \"{0}\" left bound position missing!", name);
                }
#endif                
                if (rightBoundPosition != null)
                {
                    ret.y = rightBoundPosition.position.x;
                    if (!leftStartPosition)
                    {
                        ret.z = ret.y;
                    }
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogErrorFormat("For \"{0}\" right bound position missing!", name);
                }
#endif                
                
                return ret;
            }
        }

        public override Vector3 boundCamera
        {
            get
            {
                var ret = Vector3.zero;

                if (leftBoundCamera != null)
                {
                    ret.x = leftBoundCamera.position.x;
                    if (leftStartPosition)
                    {
                        ret.z = ret.x;
                    }
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogErrorFormat("For \"{0}\" left bound camera position missing!", name);
                }
#endif                
                if (rightBoundCamera != null)
                {
                    ret.y = rightBoundCamera.position.x;
                    if (!leftStartPosition)
                    {
                        ret.z = ret.y;
                    }
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogErrorFormat("For \"{0}\" right bound camera position missing!", name);
                }
#endif                
                return ret;
            }
        }


    }
	
}
