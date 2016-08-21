// <copyright file="ListBox.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace V4F.Prototype.UI
{

    public class ListBox : MonoBehaviour
    {        
        public Vector2 sizeCell;
        public ListBoxDirection direction = ListBoxDirection.LeftRight;
        public float margin;
        public float divider;
        public int availableCount = 4;

        private RectTransform _rect;
        private RectTransform _rectAnchor;
        private List<Vector2> _anchors;
        private Vector2 _tweakPosition;
        private Vector2 _capturePosition;
        private Vector2 _afterPosition;
        private Vector2 _beforPosition;
        private bool _captureScroll;

        private IEnumerator _tweaking = null;
        private float _step = 0f;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _anchors = new List<Vector2>(20);
        }

        private void OnEnable()
        {
            TouchAdapter.OnTouchDown += TouchDownHandler;
            TouchAdapter.OnTouchMove += TouchMoveHandler;
            TouchAdapter.OnTouchTap += TouchTapHandler;
            TouchAdapter.OnTouchUp += TouchUpHandler;
        }

        private void OnDisable()
        {
            TouchAdapter.OnTouchDown -= TouchDownHandler;
            TouchAdapter.OnTouchMove -= TouchMoveHandler;
            TouchAdapter.OnTouchTap -= TouchTapHandler;
            TouchAdapter.OnTouchUp -= TouchUpHandler;
        }

        private void Start()
        {
            var anchor = new GameObject("anchor");
            _rectAnchor = anchor.AddComponent<RectTransform>();
            _rectAnchor.SetParent(_rect, false);
            direction.SetupAnchor(ref _rectAnchor);

            var cellPosition = direction.GetFirstPosition(sizeCell, margin);
            var anchorPosition = _rectAnchor.anchoredPosition;
            for (var i = 0; i < 8; ++i)
            {
                if (!(i < availableCount))
                {
                    anchorPosition += direction.GetAnchorOffset(sizeCell, divider);
                }

                _anchors.Add(anchorPosition);
                Debug.Log(i + ") " + anchorPosition);

                var cell = AddCell(i);
                cell.SetParent(_rectAnchor, false);
                cell.anchoredPosition = cellPosition;                

                cellPosition += direction.GetOffset(sizeCell, divider);                
            }
        }

        private RectTransform AddCell(int i)
        {
            var cell = new GameObject(string.Format("cell{0}", i));

            var rect = cell.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = sizeCell;

            var image = cell.AddComponent<Image>();
            image.raycastTarget = false;

            return rect;
        }

        private void TouchDownHandler(TouchAdapter sender, TouchEventArgs args)
        {
            _captureScroll = RectTransformUtility.RectangleContainsScreenPoint(_rect, args.position, sender.camera);
            if (_captureScroll)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, args.position, sender.camera, out _beforPosition);
                _capturePosition = _rectAnchor.anchoredPosition;
            }
        }

        private void TouchUpHandler(TouchAdapter sender, TouchEventArgs args)
        {
            _captureScroll = false;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, args.position, sender.camera, out _afterPosition);
            var target = _capturePosition + direction.DeltaTransform(_beforPosition, _afterPosition);
            var index = direction.GetClosestAnchor(_anchors, target, out target);
            PlayTweaking(target, false, true);

            Debug.Log(index);
            Debug.Log(Mathf.Clamp(index - availableCount + 1, 0, 8));
        }

        private void TouchMoveHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (_captureScroll)
            {                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, args.position, sender.camera, out _afterPosition);
                var target = _capturePosition + direction.DeltaTransform(_beforPosition, _afterPosition);
                PlayTweaking(target, true, false);
            }            
        }

        private void TouchTapHandler(TouchAdapter sender, TouchEventArgs args)
        {

        }

        protected void PlayTweaking(Vector2 target, bool inMoving, bool closest)
        {
            var tweaking = (closest ? 0f : divider * 1.5f);

            _tweakPosition = direction.ClampTransform(target, _anchors[0], _anchors[_anchors.Count - 1], tweaking);

            if (!inMoving)
            {
                _step = 0f;
            }           

            if (_tweaking == null)
            {
                _tweaking = Tweaking(_rectAnchor);
                StartCoroutine(_tweaking);
            }
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
    }
	
}
