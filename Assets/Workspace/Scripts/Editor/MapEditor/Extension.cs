// <copyright file="Extension.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEditor;

using V4F.Prototype.Map;

namespace V4F.MapEditor
{

    public static class Extension
    {
        public static bool Exists(this ITool self)
        {
            return (self != null);
        }

        public static int PointToIndex(this Data self, int x, int y)
        {
            return y * self.width + x;
        }

        public static void IndexToPoint(this Data self, int index, out int x, out int y)
        {
            x = index % self.width / 2;
            y = index / self.width / 2;
        }

        public static bool IndexToPoints(this Data self, int index, out int x1, out int y1, out int x2, out int y2)
        {
            var x = index % self.width;
            var y = index / self.width;
            var even = ((x % 2) == 0);

            if ((y % 2) == 1)
            {
                if (even)
                {
                    x1 = x / 2;
                    x2 = x1;
                    y1 = (y - 1) / 2;
                    y2 = (y + 1) / 2;
                    return true;
                }                
            }
            else if (!even)
            {
                x1 = (x - 1) / 2;
                x2 = (x + 1) / 2;
                y1 = y / 2;
                y2 = y1;
                return true;
            }

            x1 = x;
            x2 = x;
            y1 = y;
            y2 = y;
            return false;
        }

        public static bool IsRoomExists(this Data self, int x, int y)
        {
            var index = self.PointToIndex(x, y) * 2;
            if (index == Mathf.Clamp(index, 0, self.length - 1))
            {
                return (self[index].type == NodeType.Room);
            }
            return false;
        }

        public static Node GetRoom(this Data self, int x, int y)
        {
            var index = self.PointToIndex(x, y) * 2;
            if (index == Mathf.Clamp(index, 0, self.length - 1))
            {
                return self[index];
            }
            return null;
        }

        public static bool AddRoom(this Data self, int x, int y, out Node node)
        {
            var index = self.PointToIndex(x, y) * 2;
            if (index == Mathf.Clamp(index, 0, self.length - 1))
            {
                var prev = self[index];
                if (prev.type == NodeType.None)
                {
                    node = new Node(NodeType.Room, index);
                    self[index] = node;
                    return true;
                }
            }

            node = null;
            return false;
        }

        public static bool RemoveRoom(this Data self, int x, int y, out Node node)
        {
            var index = self.PointToIndex(x, y) * 2;
            if (index == Mathf.Clamp(index, 0, self.length - 1))
            {
                var prev = self[index];
                if (prev.type == NodeType.Room)
                {
                    node = prev;
                    self[index] = new Node(NodeType.None, index);
                    return true;
                }
            }

            node = null;
            return false;
        }

        public static bool AddTransition(this Data self, int x1, int y1, int x2, int y2, out Node node)
        {
            var dx = Mathf.Abs(x1 - x2);
            var dy = Mathf.Abs(y1 - y2);
            
            if ((dx + dy) == 1)
            {
                var index1 = self.PointToIndex(x1, y1) * 2;
                if (index1 == Mathf.Clamp(index1, 0, self.length - 1))
                {
                    var index2 = self.PointToIndex(x2, y2) * 2;
                    if (index2 == Mathf.Clamp(index2, 0, self.length - 1))
                    {
                        if ((self[index1] != null) && (self[index2] != null))
                        {
                            var x = x2 + x1;
                            var y = y2 + y1;

                            var index = y * self.width + x;
                            if (index == Mathf.Clamp(index, 0, self.length - 1))
                            {
                                Node prev = self[index];
                                if (prev.type == NodeType.None)
                                {
                                    node = new Node(((dx < dy) ? NodeType.TransitionVer : NodeType.TransitionHor), index);
                                    self[index] = node;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }            

            node = null;
            return false;
        }

        public static bool RemoveTransition(this Data self, int x1, int y1, int x2, int y2, out Node node)
        {
            var dx = Mathf.Abs(x1 - x2);
            var dy = Mathf.Abs(y1 - y2);

            if ((dx + dy) == 1)
            {
                var index1 = self.PointToIndex(x1, y1) * 2;
                if (index1 == Mathf.Clamp(index1, 0, self.length - 1))
                {
                    var index2 = self.PointToIndex(x2, y2) * 2;
                    if (index2 == Mathf.Clamp(index2, 0, self.length - 1))
                    {
                        if ((self[index1] != null) && (self[index2] != null))
                        {
                            var x = x2 + x1;
                            var y = y2 + y1;

                            var index = y * self.width + x;
                            if (index == Mathf.Clamp(index, 0, self.length - 1))
                            {
                                Node prev = self[index];
                                if (prev.type != NodeType.None)
                                {
                                    node = prev;
                                    self[index] = new Node(NodeType.None, index);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            node = null;
            return false;
        }
    }
	
}
