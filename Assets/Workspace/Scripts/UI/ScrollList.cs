// <copyright file="ScrollList.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace V4F.UI
{

    public class ScrollList<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Types
        public enum Direction
        {
            Horizontal,
            Vertical,
        }
        #endregion

        #region Fields
        //[SerializeField, HideInInspector]
        public RectTransform container = null;

        //[SerializeField, HideInInspector]
        public GameObject cellPrefab = null;

        [SerializeField, HideInInspector]
        protected Direction direction = Direction.Horizontal;

        [SerializeField, HideInInspector]
        protected float smooth = 20f;

        [SerializeField, HideInInspector]
        protected float between = 32f;

        [SerializeField, HideInInspector]
        protected float margin = 95f;

        private IEnumerator         _tweaking;
        private RectTransform       _transform;
        private RectTransform       _cellRect;
        private List<Vector2>       _anchors;
        private List<T>             _items;
        private List<RectTransform> _rects;
        private Vector2             _lastPosition;
        private Vector2             _basePosition;
        private Vector2             _capturePosition;
        private Vector2             _targetPosition;
        private Vector2             _beforPosition;
        private Vector2             _afterPosition;
        private int                 _maximum;
        private int                 _current;
        private bool                _capture;        
        private bool                _move;
        #endregion

        #region Properties
        protected new RectTransform transform
        {
            get { return _transform; }
        }

        protected T this[int index]
        {
            get { return _items[index]; }
        }

        private int lastIndex
        {
            get { return Mathf.Max(_items.Count - _maximum, 0); }
        }

        private float cellSize
        {
            get { return _cellRect.sizeDelta[(int)direction] + between; }
        }

        private bool capture
        {
            get { return _capture; }

            set
            {
                if (_capture && !value)
                {
                    var velocity = (_afterPosition - _beforPosition);
                    var speed = velocity.sqrMagnitude;
                    velocity.Normalize();

                    var delta = ((speed > 400f) ? ((velocity[(int)direction] > 0f) ? -1 : 1) : 0);
                    _current = Mathf.Clamp(Mathf.RoundToInt(Mathf.Abs(_basePosition[(int)direction] - container.anchoredPosition[(int)direction]) / cellSize) + delta, 0, lastIndex);
                    _targetPosition = _anchors[_current];

                    _tweaking = Tweaking();
                    StartCoroutine(_tweaking);
                }
                else if (!_capture && value)
                {
                    if (_tweaking != null)
                    {
                        StopCoroutine(_tweaking);
                        _tweaking = null;
                    }
                }

                _capture = value;
                _move = false;                
            }
        }
        #endregion

        #region Methods
        public T AddItem()
        {            
            var cell = Instantiate(cellPrefab);

            _anchors.Add(_basePosition - _lastPosition);

            var item = cell.AddComponent<T>();
            _items.Add(item);

            var rect = cell.GetComponent<RectTransform>();
            rect.SetParent(container, false);
            rect.anchoredPosition = _lastPosition;
            _rects.Add(rect);            

            var x = _basePosition[(int)direction] + _lastPosition[(int)direction];
            var w = _transform.rect.position[(int)direction] + _transform.rect.size[(int)direction];
            if (x < w)
            {
                ++_maximum;
            }            

            _lastPosition[(int)direction] += cellSize;

            return item;
        }

        protected virtual bool OnCaptureItem(int index, Vector2 screenPoint, Camera camera)
        {
            return false;
        }

        private void TouchLongHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (_capture)
            {
                _beforPosition = _afterPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_transform, args.position, sender.camera, out _afterPosition);

                var count = Mathf.Min(_current + _maximum, _items.Count);
                for (var i = 0; i < count; ++i)
                {
                    var index = _current + i;

                    //var item = _items[index];
                    var rect = _rects[index];
                    if (RectTransformUtility.RectangleContainsScreenPoint(rect, args.position, sender.camera))
                    {
                        capture = !OnCaptureItem(index, args.position, sender.camera);
                        break;
                    }
                }                
            }
        }

        private void TouchDownHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (!(_items.Count > 0))
            {
                return;
            }

            capture = RectTransformUtility.RectangleContainsScreenPoint(_transform, args.position, sender.camera);
            if (capture)
            {
                _capturePosition = container.anchoredPosition;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(_transform, args.position, sender.camera, out _beforPosition);
                _afterPosition = _beforPosition;                
            }
        }

        private void TouchMoveHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (capture)
            {                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_transform, args.position, sender.camera, out _afterPosition);

                var offset = _capturePosition[(int)direction] + (_afterPosition - _beforPosition)[(int)direction];
                _targetPosition.x = Mathf.Clamp(offset, _anchors[lastIndex][(int)direction] - margin, _anchors[0][(int)direction] + margin);

                _move = true;
            }            
        }

        private void TouchPressHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (capture && _move)
            {
                container.anchoredPosition = Vector2.Lerp(container.anchoredPosition, _targetPosition, smooth * Time.deltaTime);

                if (Vector2.Distance(container.anchoredPosition, _targetPosition) < 1f)
                {
                    container.anchoredPosition = _targetPosition;
                    _move = false;
                }
            }
        }

        private void TouchUpHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (capture)
            {
                _beforPosition = _afterPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_transform, args.position, sender.camera, out _afterPosition);

                capture = false;
            }            
            
        }

        private IEnumerator Tweaking()
        {
            var step = 0f;
            while (step < 1f)
            {
                step = Mathf.Clamp01(step + smooth * Time.deltaTime * 0.5f);
                container.anchoredPosition = Vector2.Lerp(container.anchoredPosition, _targetPosition, step);
                yield return null;
            }

            container.anchoredPosition = _targetPosition;

            _tweaking = null;
        }

        protected virtual void Awake()
        {
            _transform = GetComponent<RectTransform>();
            _cellRect = cellPrefab.GetComponent<RectTransform>();

            _anchors = new List<Vector2>(30);
            _items = new List<T>(30);
            _rects = new List<RectTransform>(30);

            _basePosition = container.anchoredPosition;
            _lastPosition = Vector2.zero;

            _current = 0;
        }

        protected virtual void OnEnable()
        {
            TouchAdapter.OnTouchLong += TouchLongHandler;
            TouchAdapter.OnTouchDown += TouchDownHandler;
            TouchAdapter.OnTouchMove += TouchMoveHandler;
            TouchAdapter.OnTouchPress += TouchPressHandler;
            TouchAdapter.OnTouchUp += TouchUpHandler;
        }

        protected virtual void OnDisable()
        {
            TouchAdapter.OnTouchLong -= TouchLongHandler;
            TouchAdapter.OnTouchDown -= TouchDownHandler;
            TouchAdapter.OnTouchMove -= TouchMoveHandler;
            TouchAdapter.OnTouchPress -= TouchPressHandler;
            TouchAdapter.OnTouchUp -= TouchUpHandler;
        }
        #endregion
    }

}
