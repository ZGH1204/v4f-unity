// <copyright file="ListBoxExtension.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

namespace V4F.Prototype.UI
{

    public static class ListBoxExtension
    {
        #region Methods
        public static void SetupAnchor(this ListBoxDirection self, ref RectTransform rect)
        {
            switch (self)
            {
                case ListBoxDirection.LeftRight:
                    rect.anchorMin = new Vector2(0f, 0.5f);
                    rect.anchorMax = new Vector2(0f, 0.5f);
                    break;
                case ListBoxDirection.TopBottom:
                    rect.anchorMin = new Vector2(0.5f, 1f);
                    rect.anchorMax = new Vector2(0.5f, 1f);
                    break;
                case ListBoxDirection.RightLeft:
                    rect.anchorMin = new Vector2(1f, 0.5f);
                    rect.anchorMax = new Vector2(1f, 0.5f);
                    break;
                case ListBoxDirection.BottomTop:
                    rect.anchorMin = new Vector2(0.5f, 0f);
                    rect.anchorMax = new Vector2(0.5f, 0f);                    
                    break;
            }

            rect.sizeDelta = new Vector2(0f, 0f);
        }

        public static Vector2 GetFirstPosition(this ListBoxDirection self, Vector2 size, float margin)
        {
            switch (self)
            {
                case ListBoxDirection.LeftRight:
                    return new Vector2(margin + size.x * 0.5f, 0f);
                case ListBoxDirection.TopBottom:
                    return new Vector2(0f, -(margin + size.y * 0.5f));
                case ListBoxDirection.RightLeft:
                    return new Vector2(-(margin + size.x * 0.5f), 0f);
                case ListBoxDirection.BottomTop:
                    return new Vector2(0f, margin + size.y * 0.5f);                    
                default:
                    return Vector2.zero;
            }
        }

        public static Vector2 GetOffset(this ListBoxDirection self, Vector2 size, float divider)
        {
            switch (self)
            {
                case ListBoxDirection.LeftRight:
                    return new Vector2(divider + size.x, 0f);
                case ListBoxDirection.TopBottom:
                    return new Vector2(0f, -(divider + size.y));
                case ListBoxDirection.RightLeft:
                    return new Vector2(-(divider + size.x), 0f);
                case ListBoxDirection.BottomTop:
                    return new Vector2(0f, divider + size.y);                    
                default:
                    return Vector2.zero;
            }
        }

        public static Vector2 GetAnchorOffset(this ListBoxDirection self, Vector2 size, float divider)
        {
            switch (self)
            {
                case ListBoxDirection.LeftRight:
                    return new Vector2(-(divider + size.x), 0f);
                case ListBoxDirection.TopBottom:
                    return new Vector2(0f, divider + size.y);
                case ListBoxDirection.RightLeft:
                    return new Vector2(divider + size.x, 0f);
                case ListBoxDirection.BottomTop:
                    return new Vector2(0f, -(divider + size.y));
                default:
                    return Vector2.zero;
            }
        }

        public static Vector2 DeltaTransform(this ListBoxDirection self, Vector2 befor, Vector2 after)
        {
            switch (self)
            {
                case ListBoxDirection.LeftRight:
                case ListBoxDirection.RightLeft:
                    return new Vector2(after.x - befor.x, 0f);
                case ListBoxDirection.TopBottom:
                case ListBoxDirection.BottomTop:
                    return new Vector2(0f, after.y - befor.y);
                default:
                    return Vector2.zero;
            }
        }

        public static Vector2 ClampTransform(this ListBoxDirection self, Vector2 position, Vector2 first, Vector2 last, float tweaking)
        {
            switch (self)
            {
                case ListBoxDirection.LeftRight:
                    return new Vector2(Mathf.Clamp(position.x, last.x - tweaking, first.x + tweaking), position.y);
                case ListBoxDirection.TopBottom:
                    return new Vector2(position.x, Mathf.Clamp(position.y, first.y - tweaking, last.y + tweaking));
                case ListBoxDirection.RightLeft:
                    return new Vector2(Mathf.Clamp(position.x, first.x - tweaking, last.x + tweaking), position.y);
                case ListBoxDirection.BottomTop:
                    return new Vector2(position.x, Mathf.Clamp(position.y, last.y - tweaking, first.y + tweaking));
                default:
                    return Vector2.zero;
            }
        }

        public static int GetClosestAnchor(this ListBoxDirection self, List<Vector2> anchors, Vector2 position, out Vector2 closest)
        {
            closest = position;

            var p = ((self == ListBoxDirection.TopBottom || self == ListBoxDirection.BottomTop) ? 1 : 0);
            var d = 5000f;
            var i = -1;   
                     
            for (var j = 0; j < anchors.Count; ++j)
            {                
                var delta = Mathf.Abs(anchors[j][p] - position[p]);
                if (delta < d)
                {
                    d = delta;
                    i = j;
                }
            }

            if (i != -1)
            {
                closest = anchors[i];
            }

            return i;
        }
        #endregion
    }

}
