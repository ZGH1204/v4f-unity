// <copyright file="PageView.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

namespace V4F.UI
{

    public class PageView : Handler
    {
        #region Types
        public delegate void PageViewEventHandler(PageView sender, PageViewEventArgs args);
        #endregion

        #region Events
        public event PageViewEventHandler OnScrollBegin;
        public event PageViewEventHandler OnScrollEnd;
        #endregion

        #region Fields        
        public RectTransform prevAnchor;
        public RectTransform currAnchor;
        public RectTransform nextAnchor;
        public RectTransform[] pages;
        public PageBar pageBar;

        [Range(1f, 100f)]
        public float scrollMultiplier = 1f;

        private RectTransform _rect;
        private Vector2 _anchorPosition;
        private Vector2[] _tweakPosition;
        private Vector2 _capturePosition;
        private Vector2 _afterPosition;
        private Vector2 _beforPosition;
        private float[] _currToPrev;
        private float[] _currToNext;
        private int _anchorIndex;
        private int _selectIndex;
        private int _waitIndex;
        private bool _captureScroll;
        private bool _captureLock;

        private IEnumerator _tweaking = null;
        private IEnumerator _scroll = null;
        private float _step;
        #endregion

        #region Properties
        public RectTransform prevPage
        {
            get
            {
                var prevIndex = _selectIndex - 1;
                return (!(prevIndex < 0) ? pages[prevIndex] : null);
            }
        }

        public RectTransform currPage
        {
            get { return pages[_selectIndex]; }
        }

        public RectTransform nextPage
        {
            get
            {
                var nextIndex = _selectIndex + 1;
                return ((nextIndex < pages.Length) ? pages[nextIndex] : null);
            }
        }
        #endregion

        #region Methods
        protected virtual void Awake()
        {
            _rect = GetComponent<RectTransform>();            

            _tweakPosition = new Vector2[] { Vector2.zero, Vector2.zero, Vector2.zero };

            _currToPrev = new float[] { 1f, 1f };
            _currToPrev[0] = Mathf.Abs((currAnchor.anchoredPosition - prevAnchor.anchoredPosition).x);
            _currToPrev[1] = 1f / _currToPrev[0];

            _currToNext = new float[] { 1f, 1f };
            _currToNext[0] = Mathf.Abs((currAnchor.anchoredPosition - nextAnchor.anchoredPosition).x);
            _currToNext[1] = 1f / _currToNext[0];

            _tweaking = null;
            _selectIndex = 0;
            _captureScroll = false;
            _captureLock = false;            
        }

        protected virtual void OnEnable()
        {
            input.OnTouchDown += TouchDownHandler;
            input.OnTouchMove += TouchMoveHandler;
            input.OnTouchUp += TouchUpHandler;

            pageBar.OnClick += OnPageBarClick;            
        }

        protected virtual void OnDisable()
        {
            input.OnTouchDown -= TouchDownHandler;
            input.OnTouchMove -= TouchMoveHandler;
            input.OnTouchUp -= TouchUpHandler;

            pageBar.OnClick -= OnPageBarClick;            
        }

        protected virtual void Start()
        {
            for (var i = 0; i < pages.Length; ++i)
            {
                pages[i].anchoredPosition = nextAnchor.anchoredPosition;
            }

            pageBar.Select(_selectIndex);
            PlayTweaking(false, true);
        }

        private void OnScrollBeginCallback(PageViewEventArgs args)
        {
            if (OnScrollBegin != null)
            {
                OnScrollBegin(this, args);
            }
        }

        private void OnScrollEndCallback(PageViewEventArgs args)
        {
            if (OnScrollEnd != null)
            {
                OnScrollEnd(this, args);
            }
        }

        private void TouchDownHandler(TouchAdapter sender, TouchEventArgs args)
        {
            _captureScroll = (!_captureLock) && RectTransformUtility.RectangleContainsScreenPoint(_rect, args.position, sender.camera);
            if (_captureScroll)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(currPage, args.position, sender.camera))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, args.position, sender.camera, out _beforPosition);
                    _capturePosition = currPage.anchoredPosition;
                    StopTweaking();

                    var eventArgs = new PageViewEventArgs();
                    OnScrollBeginCallback(eventArgs);
                }
                else
                {
                    _captureScroll = false;
                }                
            }
        }

        private void TouchUpHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (_captureScroll)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, args.position, sender.camera, out _afterPosition);

                var delta = new Vector2(_afterPosition.x - _beforPosition.x, 0f);
                var target = _capturePosition + delta;
                
                if (Mathf.Abs(target.x - _capturePosition.x) > (currPage.sizeDelta.x * 0.65f))
                {
                    if (target.x < _capturePosition.x)
                    {
                        _selectIndex = Mathf.Clamp(_selectIndex + 1, 0, pages.Length - 1);
                    }
                    else
                    {
                        _selectIndex = Mathf.Clamp(_selectIndex - 1, 0, pages.Length - 1);                        
                    }

                    pageBar.Select(_selectIndex);
                }

                PlayTweaking(false, true);
                
                _captureScroll = false;

                var eventArgs = new PageViewEventArgs();
                OnScrollEndCallback(eventArgs);
            }
        }

        private void TouchMoveHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (_captureScroll)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, args.position, sender.camera, out _afterPosition);

                var delta = new Vector2(_afterPosition.x - _beforPosition.x, 0f);
                var target = _capturePosition + delta;

                if (target.x < _capturePosition.x)
                {
                    if (_selectIndex == (pages.Length - 1))
                    {
                        delta.x = -currPage.sizeDelta.x * 0.5f;
                        target = _capturePosition + delta;
                    }
                    else
                    {
                        var distance = Mathf.Abs(delta.x);
                        delta.x = (distance * _currToPrev[1]) * _currToNext[0] * (delta.x / distance);
                        _tweakPosition[2] = nextAnchor.anchoredPosition + delta;
                        _tweakPosition[1] = prevAnchor.anchoredPosition;
                    }
                }
                else if (_capturePosition.x < target.x)
                {
                    if (_selectIndex == 0)
                    {
                        delta.x = currPage.sizeDelta.x * 0.5f;
                        target = _capturePosition + delta;
                    }
                    else
                    {
                        var distance = Mathf.Abs(delta.x);
                        delta.x = (distance * _currToNext[1]) * _currToPrev[0] * (delta.x / distance);
                        _tweakPosition[1] = prevAnchor.anchoredPosition + delta;
                        _tweakPosition[2] = nextAnchor.anchoredPosition;
                    }                        
                }

                _tweakPosition[0] = target;

                PlayTweaking(true, false);
            }
        }

        private void OnPageBarClick(PageBar sender, PageBarEventArgs args)
        {
            var index = args.index;
            if (_selectIndex != index)
            {
                if (!(index < 0) && (index < pages.Length))
                {
                    _waitIndex = index;
                    if (_scroll == null)
                    {
                        _scroll = Scroll();
                        StartCoroutine(_scroll);
                    }                    
                }                
            }
        }

        private void PlayTweaking(bool inMoving, bool closest)
        {            
            if (!inMoving)
            {
                _step = 0f;
            }

            if (closest)
            {
                _tweakPosition[0] = currAnchor.anchoredPosition;
                _tweakPosition[1] = prevAnchor.anchoredPosition;
                _tweakPosition[2] = nextAnchor.anchoredPosition;
            }
            
            if (_tweaking == null)
            {
                _tweaking = Tweaking();
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
        }

        private IEnumerator Tweaking(float scaleTime = 1f, bool express = false)
        {
            RectTransform rect = null;

            var roof = (express ? 0.5f : 1f);
            _step = (express ? 0f : 0f);
            
            while (_step < roof)
            {
                _step = Mathf.Clamp01(_step + Time.smoothDeltaTime * scaleTime);

                rect = currPage;
                if (rect != null)
                {
                    rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, _tweakPosition[0], _step);
                }

                rect = prevPage;
                if (rect != null)
                {
                    rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, _tweakPosition[1], _step);
                }

                rect = nextPage;
                if (rect != null)
                {
                    rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, _tweakPosition[2], _step);
                }

                yield return null;
            }

            _tweaking = null;

            if (!express)
            {
                rect = currPage;
                if (rect != null)
                {
                    rect.anchoredPosition = _tweakPosition[0];
                }

                rect = prevPage;
                if (rect != null)
                {
                    rect.anchoredPosition = _tweakPosition[1];
                }

                rect = nextPage;
                if (rect != null)
                {
                    rect.anchoredPosition = _tweakPosition[2];
                }
            }            
        }

        private IEnumerator Scroll()
        {
            _captureLock = true;

            var step = ((_waitIndex < _selectIndex) ? -1 : 1);

            while (_selectIndex != _waitIndex)
            {
                _selectIndex += step;
                yield return StartCoroutine(Tweaking(scrollMultiplier, (_selectIndex != _waitIndex)));
                step = ((_waitIndex < _selectIndex) ? -1 : 1);
            }

            _scroll = null;

            _captureLock = false;
        }
        #endregion
    }

}
