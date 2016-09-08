// <copyright file="MapController.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using V4F.Prototype.Dungeon;
using V4F.Prototype.Map;
using V4F.UI;

namespace V4F.Prototype.UI
{

    public class MapController : MonoBehaviour
    {
        //public Manager manager;
        public Data data;
        public RectTransform root;
        public RectTransform prefabRoom;
        public RectTransform prefabTransitionHor;
        public RectTransform prefabTransitionVer;
        public RectTransform dragRect;        
        public Button alignButton;

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

        private void TouchDownHandler(TouchAdapter sender, TouchEventArgs args)
        {
            _captureScroll = !RectTransformUtility.RectangleContainsScreenPoint(alignButton.rect, args.position, sender.camera);
            _captureScroll = _captureScroll && RectTransformUtility.RectangleContainsScreenPoint(dragRect, args.position, sender.camera);
            if (_captureScroll)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(dragRect, args.position, sender.camera, out _beforPosition);
                _capturePosition = root.anchoredPosition;
                StopTweaking();
            }
        }

        private void TouchUpHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (_captureScroll)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(dragRect, args.position, sender.camera, out _afterPosition);
                var target = _capturePosition + (_afterPosition - _beforPosition);
                PlayTweaking(target, false);
                _captureScroll = false;
            }
        }

        private void TouchMoveHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (_captureScroll)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(dragRect, args.position, sender.camera, out _afterPosition);
                var target = _capturePosition + (_afterPosition - _beforPosition);
                PlayTweaking(target, true);
            }
        }

        private void TouchTapHandler(TouchAdapter sender, TouchEventArgs args)
        {

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

        private void OnAlignClick(Button sender, ButtonEventArgs args)
        {
            var target = Vector2.zero;
            PlayTweaking(target, false);
        }

        private void Awake()
        {
            _map = new Dictionary<int, RectTransform>();

            _offsetHor = Vector2.zero;
            _offsetVer = Vector2.zero;

            //var data = manager.map;
            var delta = Data.RowCount / 2;
            for (var i = 0; i < data.length; ++i)
            {
                var node = data[i];
                if (node.type == NodeType.None)
                {
                    continue;
                }

                var x = i % data.width;
                var y = i / data.width;
                var anchorPosition = new Vector2((x - delta) * 198f, -(y - delta) * 198f);

                if (_offsetHor.x > anchorPosition.x)
                {
                    _offsetHor.x = anchorPosition.x;
                }
                if (_offsetHor.y < anchorPosition.x)
                {
                    _offsetHor.y = anchorPosition.x;
                }

                if (_offsetVer.x > anchorPosition.y)
                {
                    _offsetVer.x = anchorPosition.y;
                }
                if (_offsetVer.y < anchorPosition.y)
                {
                    _offsetVer.y = anchorPosition.y;
                }

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

                var transform = instance.GetComponent<RectTransform>();
                transform.SetParent(root, false);
                transform.anchoredPosition = anchorPosition;

                _map.Add(node.index, transform);
            }

            _offsetHor = new Vector2(-_offsetHor.y, -_offsetHor.x);
            _offsetVer = new Vector2(-_offsetVer.y, -_offsetVer.x);
        }

        private void OnEnable()
        {
            TouchAdapter.OnTouchDown += TouchDownHandler;
            TouchAdapter.OnTouchMove += TouchMoveHandler;
            TouchAdapter.OnTouchTap += TouchTapHandler;
            TouchAdapter.OnTouchUp += TouchUpHandler;

            alignButton.OnClick += OnAlignClick;
        }

        private void OnDisable()
        {
            TouchAdapter.OnTouchDown -= TouchDownHandler;
            TouchAdapter.OnTouchMove -= TouchMoveHandler;
            TouchAdapter.OnTouchTap -= TouchTapHandler;
            TouchAdapter.OnTouchUp -= TouchUpHandler;

            alignButton.OnClick -= OnAlignClick;
        }        
    }
	
}