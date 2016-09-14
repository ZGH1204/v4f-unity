// <copyright file="Location.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

using V4F.UI;
using V4F.UI.Map;
using V4F.Prototype.Map;

namespace V4F.Prototype.Mission
{
    
    public class Location : MonoBehaviour
    {        
        public List<Material> corridors;
        public List<Material> rooms;

        public RectTransform prefabRoom;
        public RectTransform prefabTransitionHor;
        public RectTransform prefabTransitionVer;
        public MapHandler mapHandler;
        public Button enterButton;
        public Button exitButton;

        private Dictionary<int, NodeContent> _content;
        private NodeMap[] _map;
        private int _length;
        private int _width;
        private int _half;
        private int _position;
        private int _lastPosition;
        private int _nextPosition;

        private bool _changeRoomTrigger;

        public int position
        {
            get { return _position; }
        }

        public NodeMap current
        {
            get { return _map[_position]; }
        }

        public NodeContent currentContent
        {
            get { return _content[_position]; }
        }

        public bool changeRoomTrigger
        {
            get
            {
                if (_changeRoomTrigger)
                {
                    _changeRoomTrigger = false;
                    return true;
                }
                return false;
            }

            private set { _changeRoomTrigger = value; }
        }

        public NodeMap this[int position]
        {
            get
            {
                if (position == Mathf.Clamp(position, 0, _length - 1))
                {
                    return _map[position];
                }
                return null;
            }
        }

        public NodeMap this[int x, int y]
        {
            get
            {
                var position = y * _width + x;
                if (position == Mathf.Clamp(position, 0, _length -1))
                {
                    return _map[position];
                }
                return null;
            }
        }        

        public NodeContent GetContent(int position)
        {
            NodeContent value = null;
            if (_content.TryGetValue(position, out value))
            {
                return value;
            }
            return null;
        }

        public Material GetMaterial(NodeType type, int index)
        {
            if (type == NodeType.Room)
            {
                return rooms[index];
            }
            else if (type != NodeType.None)
            {
                return corridors[index];
            }
            return null;
        }

        public bool GetRoomIndices(out List<int> indices)
        {
            indices = new List<int>(4);

            var node = current;
            if (node.type == NodeType.Room)
            {
                var x = _position % _width;
                var y = _position / _width;

                var i = (y + 2) * _width + x;
                if (i == Mathf.Clamp(i, 0, _length - 1))
                {
                    var j = (y + 1) * _width + x;
                    var c = _map[j];
                    var r = _map[i];
                    if ((c != null) && (r != null))
                    {
                        indices.Add(i);
                    }
                }

                i = (y - 2) * _width + x;
                if (i == Mathf.Clamp(i, 0, _length - 1))
                {
                    var j = (y - 1) * _width + x;
                    var c = _map[j];
                    var r = _map[i];
                    if ((c != null) && (r != null))
                    {
                        indices.Add(i);
                    }
                }

                i = y * _width + (x + 2);
                if (i == Mathf.Clamp(i, 0, _length - 1))
                {
                    var j = y * _width + (x + 1);
                    var c = _map[j];
                    var r = _map[i];
                    if ((c != null) && (r != null))
                    {
                        indices.Add(i);
                    }
                }

                i = y * _width + (x - 2);
                if (i == Mathf.Clamp(i, 0, _length - 1))
                {
                    var j = y * _width + (x - 1);
                    var c = _map[j];
                    var r = _map[i];
                    if ((c != null) && (r != null))
                    {
                        indices.Add(i);
                    }
                }

                return (indices.Count > 0);
            }            

            return false;
        }

        private void LoadBeginHandler(int length, int width)
        {            
            _content = new Dictionary<int, NodeContent>(length);
            _map = new NodeMap[length];

            MapHandler.Initialize(mapHandler, length);

            _length = length;
            _width = width;
            _half = width / 2;
        }

        private void LoadEndHandler(int entry)
        {
            _position = entry;
            _lastPosition = _position;
            _nextPosition = _position;
        }

        private void LoadNodeHandler(Node node)
        {
            var index = node.index;

            if (node.type != NodeType.None)
            {
                int materialIndices = 0;

                if (node.type != NodeType.Room)
                {
                    materialIndices = NodeContent.SetMaterialIndex(materialIndices, 0, Random.Range(0, corridors.Count - 1));
                    materialIndices = NodeContent.SetMaterialIndex(materialIndices, 1, Random.Range(0, corridors.Count - 1));
                    materialIndices = NodeContent.SetMaterialIndex(materialIndices, 2, Random.Range(0, corridors.Count - 1));
                    materialIndices = NodeContent.SetMaterialIndex(materialIndices, 3, Random.Range(0, corridors.Count - 1));
                }
                else
                {
                    materialIndices = NodeContent.SetMaterialIndex(materialIndices, 0, Random.Range(0, rooms.Count - 1));
                }

                _content.Add(index, new NodeContent(materialIndices));
                _map[index] = new NodeMap(node.type);

                // ---------------------------------------------------------------------

                var x = index % _width;
                var y = index / _width;
                var anchoredPosition = new Vector2((x - _half) * 198f, -(y - _half) * 198f);

                GameObject instance = null;
                if (node.type == NodeType.Room)
                {
                    instance = Instantiate(prefabRoom.gameObject) as GameObject;
                }
                else if (node.type == NodeType.TransitionHor)
                {
                    instance = Instantiate(prefabTransitionHor.gameObject) as GameObject;
                }
                else if (node.type == NodeType.TransitionVer)
                {
                    instance = Instantiate(prefabTransitionVer.gameObject) as GameObject;
                }

                var location = instance.GetComponent<RectTransform>();
                location.anchoredPosition = anchoredPosition;                

                MapHandler.AddLocation(mapHandler, location, index);
            }
        }

        private void MapChooseRoomHandler(MapHandler sender, MapEventArgs args)
        {
            _nextPosition = args.chooseRoomIndex;
            _lastPosition = _position;

            var x1 = _lastPosition % _width;
            var y1 = _lastPosition / _width;
            var x2 = _nextPosition % _width;
            var y2 = _nextPosition / _width;

            var x = (x1 + x2) / 2;
            var y = (y1 + y2) / 2;

            _position = y * _width + x;
            _changeRoomTrigger = true;
        }

        private void EnterClickHandler(Button sender, ButtonEventArgs args)
        {
            var position = _lastPosition;

            _lastPosition = position;
            _nextPosition = position;

            _position = position;
            _changeRoomTrigger = true;
        }

        private void ExitClickHandler(Button sender, ButtonEventArgs args)
        {
            var position = _nextPosition;

            _lastPosition = position;
            _nextPosition = position;

            _position = position;
            _changeRoomTrigger = true;
        }

        private void OnEnable()
        {
            Loading.OnBegin += LoadBeginHandler;
            Loading.OnNode += LoadNodeHandler;
            Loading.OnEnd += LoadEndHandler;

            mapHandler.OnChooseRoom += MapChooseRoomHandler;
            enterButton.OnClick += EnterClickHandler;
            exitButton.OnClick += ExitClickHandler;
        }

        private void OnDisable()
        {
            Loading.OnBegin -= LoadBeginHandler;
            Loading.OnNode -= LoadNodeHandler;
            Loading.OnEnd -= LoadEndHandler;

            mapHandler.OnChooseRoom -= MapChooseRoomHandler;
            enterButton.OnClick -= EnterClickHandler;
            exitButton.OnClick -= ExitClickHandler;
        }

    }
	
}