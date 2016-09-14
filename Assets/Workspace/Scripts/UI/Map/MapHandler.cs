// <copyright file="MapHandler.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using V4F.Prototype.Mission;
using V4F.Prototype.Map;

namespace V4F.UI.Map
{

    public class MapHandler : Handler
    {
        public delegate void EventsHandler(MapHandler sender, MapEventArgs args);

        public event EventsHandler OnChooseRoom;
        public event EventsHandler OnClosed;

        public Location location;
        public RectTransform root;
        public RectTransform drag;
        public RectTransform marker;

        private Dictionary<int, RectTransform> _map;
        private Vector2 _offsetHor;
        private Vector2 _offsetVer;
        private Vector2 _tweakPosition;
        private Vector2 _capturePosition;
        private Vector2 _afterPosition;
        private Vector2 _beforPosition;
        private bool _captureScroll;

        private IEnumerator _tweaking = null;
        private float _step = 0f;

        private List<int> _closestRoom;
        private IEnumerator _closestPulsate = null;

        public static void Initialize(MapHandler handler, int length)
        {
            handler._map = new Dictionary<int, RectTransform>(length);
        }

        public static void AddLocation(MapHandler handler, RectTransform location, int index)
        {
            var anchoredPosition = location.anchoredPosition;
            var offsetHor = handler._offsetHor;
            var offsetVer = handler._offsetVer;

            if (offsetHor.x > anchoredPosition.x)
            {
                offsetHor.x = anchoredPosition.x;
            }
            if (offsetHor.y < anchoredPosition.x)
            {
                offsetHor.y = anchoredPosition.x;
            }

            if (offsetVer.x > anchoredPosition.y)
            {
                offsetVer.x = anchoredPosition.y;
            }
            if (offsetVer.y < anchoredPosition.y)
            {
                offsetVer.y = anchoredPosition.y;
            }

            location.SetParent(handler.root, false);

            handler._offsetHor = offsetHor;
            handler._offsetVer = offsetVer;
            handler._map.Add(index, location);
        }

        private void OnChooseRoomCallback(MapEventArgs args)
        {
            if (OnChooseRoom != null)
            {
                OnChooseRoom(this, args);
            }
        }

        private void OnClosedCallback(MapEventArgs args)
        {
            if (OnClosed != null)
            {
                OnClosed(this, args);
            }
        }

        private void TouchDownHandler(TouchAdapter sender, TouchEventArgs args)
        {
            //_captureScroll = !RectTransformUtility.RectangleContainsScreenPoint(alignButton.rect, args.position, sender.camera);
            //_captureScroll = _captureScroll && RectTransformUtility.RectangleContainsScreenPoint(drag, args.position, sender.camera);
            _captureScroll = RectTransformUtility.RectangleContainsScreenPoint(drag, args.position, sender.camera);
            if (_captureScroll)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(drag, args.position, sender.camera, out _beforPosition);
                _capturePosition = root.anchoredPosition;
                StopTweaking();
            }
        }

        private void TouchUpHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (_captureScroll)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(drag, args.position, sender.camera, out _afterPosition);
                var target = _capturePosition + (_afterPosition - _beforPosition);
                PlayTweaking(target, false);
                _captureScroll = false;
            }
        }

        private void TouchMoveHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (_captureScroll)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(drag, args.position, sender.camera, out _afterPosition);
                var target = _capturePosition + (_afterPosition - _beforPosition);
                PlayTweaking(target, true);
            }
        }

        private void TouchTapHandler(TouchAdapter sender, TouchEventArgs args)
        {
            var closestCount = _closestRoom.Count;
            if (closestCount > 0)
            {
                for (var i = 0; i < closestCount; ++i)
                {
                    var index = _closestRoom[i];
                    var rect = _map[index];
                    if (RectTransformUtility.RectangleContainsScreenPoint(rect, args.position, sender.camera))
                    {
                        var eventArgs = new MapEventArgs();
                        eventArgs.currentRoomIndex = location.position;
                        eventArgs.chooseRoomIndex = index;
                        OnChooseRoomCallback(eventArgs);

                        eventArgs = new MapEventArgs();
                        OnClosedCallback(eventArgs);

                        break;
                    }                    
                }                
            }
        }

        private void BackHandler(TouchAdapter sender)
        {
            var args = new MapEventArgs();
            OnClosedCallback(args);
        }

        private void PlayTweaking(Vector2 target, bool inMoving)
        {
            _tweakPosition.x = Mathf.Clamp(target.x, _offsetHor.x, _offsetHor.y);
            _tweakPosition.y = Mathf.Clamp(target.y, _offsetVer.x, _offsetVer.y);

            if (!inMoving)
            {
                _step = 0f;
            }

            if (_tweaking == null)
            {
                _tweaking = Tweaking(root);
                StartCoroutine(_tweaking);
            }
        }

        private void StopTweaking()
        {
            if (_tweaking != null)
            {
                StopCoroutine(_tweaking);
                _tweaking = null;
            }
            _step = 0f;
        }

        private IEnumerator Tweaking(RectTransform rect)
        {
            while (_step < 1f)
            {
                _step = Mathf.Clamp01(_step + Time.smoothDeltaTime);
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, _tweakPosition, _step);
                yield return null;
            }

            rect.anchoredPosition = _tweakPosition;

            _tweaking = null;
            _step = 0f;
        }

        private void PlayClosestPulsate()
        {
            var count = _closestRoom.Count;
            for (var i = 0; i < count; ++i)
            {
                var rect = _map[_closestRoom[i]];
                rect.SetAsLastSibling();
            }

            _closestPulsate = ClosestPulsate();
            StartCoroutine(_closestPulsate);
        }

        private void StopClosestPulsate()
        {
            if (_closestPulsate != null)
            {
                StopCoroutine(_closestPulsate);
                _closestPulsate = null;

                var count = _closestRoom.Count;
                for (var i = 0; i < count; ++i)
                {
                    var rect = _map[_closestRoom[i]];
                    rect.localScale = Vector3.one;
                }
            }
        }

        private IEnumerator ClosestPulsate()
        {
            var localScale = Vector3.one;
            while (true)
            {
                var count = _closestRoom.Count;
                for (var i = 0; i < count; ++i)
                {
                    _map[_closestRoom[i]].localScale = localScale;
                }

                yield return null;

                var scale = 1f + Mathf.PingPong(Time.time, 1f) * 0.25f;
                localScale.x = scale;
                localScale.y = scale;
            }            
        }

        private void OnEnable()
        {
            input.OnTouchDown += TouchDownHandler;
            input.OnTouchMove += TouchMoveHandler;
            input.OnTouchTap += TouchTapHandler;
            input.OnTouchUp += TouchUpHandler;
            input.OnBack += BackHandler;

            if (location.GetRoomIndices(out _closestRoom))
            {
                PlayClosestPulsate();
            }

            marker.anchoredPosition = _map[location.position].anchoredPosition;
            marker.SetAsLastSibling();
        }

        private void OnDisable()
        {
            input.OnTouchDown -= TouchDownHandler;
            input.OnTouchMove -= TouchMoveHandler;
            input.OnTouchTap -= TouchTapHandler;
            input.OnTouchUp -= TouchUpHandler;
            input.OnBack -= BackHandler;

            StopClosestPulsate();
        }

        private void Start()
        {
            _offsetHor = new Vector2(-_offsetHor.y, -_offsetHor.x);
            _offsetVer = new Vector2(-_offsetVer.y, -_offsetVer.x);
        }
    }
	
}
