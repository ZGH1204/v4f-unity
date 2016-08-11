// <copyright file="ListBox.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace V4F.UI
{

    public class ListBox<T> : MonoBehaviour where T : MonoBehaviour
    {
        public delegate void ListBoxEventHandler(ListBox<T> sender, ListBoxEventArgs args);

        #region Events
        public event ListBoxEventHandler OnSelect;
        public event ListBoxEventHandler OnRemove;
        #endregion

        #region Fields
        private RectTransform _self = null;
        private RectTransform _cell = null;

        private Vector2 _capturePosition = Vector2.zero;
        private Vector2 _basePosition = Vector2.zero;
        private Vector2 _beforPosition = Vector2.zero;
        private Vector2 _afterPosition = Vector2.zero;
        private Vector2 _tweakPosition = Vector2.zero;
        private float _step = 0f;
        private int _index = -1;
        private bool _capture = false;        

        private List<T> _items = new List<T>(32);
        private List<Vector2> _points = new List<Vector2>(32);
        private List<RectTransform> _rects = new List<RectTransform>(32);
        private List<Vector2> _anchors = new List<Vector2>(32);

        private IEnumerator _tweaking = null;
        private IEnumerator _align = null;
        #endregion

        #region Properties
        public T this[int index]
        {
            get { return _items[index]; }
        }

        protected virtual GameObject prefab
        {
            get { return null; }
        }

        protected virtual RectTransform container
        {
            get { return null; }
        }

        protected virtual float margin
        {
            get { return 0f; }
        }        

        protected virtual float interval
        {
            get { return 0f; }
        }        

        protected virtual int maximum
        {
            get { return 0; }
        }

        private Vector2 delta
        {
            get { return new Vector2(_cell.sizeDelta.x + interval, 0f); }
        }

        private int lastIndex
        {
            get { return Mathf.Max(0, _items.Count - maximum); }
        }

        private bool capture
        {
            get { return _capture; }
            set
            {
                if (!_capture &&  value)
                {
                    StopTweaking();                    
                }
                else if (_capture && !value)
                {
                    var velocity = (_afterPosition - _beforPosition);
                    var offset = ((velocity.sqrMagnitude > 400f) ? ((velocity.x > 0f) ? -1 : 1) : 0);

                    _index = Mathf.Clamp(Mathf.RoundToInt(Mathf.Abs((_basePosition.x - container.anchoredPosition.x) / delta.x)) + offset, 0, lastIndex);
                    _tweakPosition = _anchors[_index];

                    PlayTweaking(false);
                }

                _capture = value;                
            }
        }
        #endregion

        #region Methods
        public T Add()
        {            
            var cell = Instantiate(prefab);

            var position = _points[_points.Count - 1];
            _points.Add(position + delta);            

            var rect = cell.GetComponent<RectTransform>();
            rect.SetParent(container, false);
            rect.anchoredPosition = position;
            _rects.Add(rect);

            var item = cell.AddComponent<T>();
            _items.Add(item);

            if (_anchors.Count < _items.Count)
            {
                _anchors.Add(_basePosition - position);
            }

            return item;
        }

        public void RemoveAt(int index)
        {
            var cell = _items[index].gameObject;

            _items.RemoveAt(index);
            _rects.RemoveAt(index);

            var last = _points[index];
            _points.RemoveAt(index);
            for (var i = index; i < _points.Count; ++i)
            {
                var next = _points[i];
                _points[i] = last;
                last = next;
            }

            DestroyObject(cell);
            PlayAlign();

            var args = new ListBoxEventArgs();
            args.index = index;
            OnRemoveCallback(args);            
        }

        protected void PlayTweaking(bool inMoving)
        {
            if (!inMoving)
            {
                _step = 0f;
            }

            if (_tweaking == null)
            {
                _tweaking = Tweaking();
                StartCoroutine(_tweaking);
            }            
        }

        protected void StopTweaking()
        {
            _step = 0f;

            if (_tweaking != null)
            {
                StopCoroutine(_tweaking);
                _tweaking = null;
            }
        }

        protected void PlayAlign()
        {
            if (_align == null)
            {
                _align = Align();
                StartCoroutine(_align);
            }            
        }

        protected void StopAlign()
        {
            if (_align != null)
            {
                StopCoroutine(_align);
                _align = null;
            }
        }

        protected virtual void Awake()
        {
            _self = GetComponent<RectTransform>();            
            _cell = prefab.GetComponent<RectTransform>();

            _basePosition = container.anchoredPosition;
            _index = 0;
            _capture = false;

            _points.Add(Vector2.zero);
        }

        protected virtual void OnEnable()
        {            
            TouchAdapter.OnTouchDown += TouchDownHandler;
            TouchAdapter.OnTouchMove += TouchMoveHandler;
            TouchAdapter.OnTouchTap += TouchTapHandler;
            TouchAdapter.OnTouchUp += TouchUpHandler;
        }

        protected virtual void OnDisable()
        {            
            TouchAdapter.OnTouchDown -= TouchDownHandler;
            TouchAdapter.OnTouchMove -= TouchMoveHandler;
            TouchAdapter.OnTouchTap -= TouchTapHandler;
            TouchAdapter.OnTouchUp -= TouchUpHandler;
        }

        private void OnSelectCallback(ListBoxEventArgs args)
        {
            if (OnSelect != null)
            {
                OnSelect(this, args);
            }
        }

        private void OnRemoveCallback(ListBoxEventArgs args)
        {
            if (OnRemove != null)
            {
                OnRemove(this, args);
            }
        }        

        private void TouchDownHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (_items.Count > 0)
            {
                capture = RectTransformUtility.RectangleContainsScreenPoint(_self, args.position, sender.camera);
                if (capture)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(_self, args.position, sender.camera, out _beforPosition);                    
                    _afterPosition = _beforPosition;
                    _capturePosition = container.anchoredPosition;
                }
            }            
        }

        private void TouchUpHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (capture)
            {
                _beforPosition = _afterPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_self, args.position, sender.camera, out _afterPosition);
            }

            capture = false;
        }

        private void TouchMoveHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (capture)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_self, args.position, sender.camera, out _afterPosition);

                var offset = _capturePosition.x + (_afterPosition.x - _beforPosition.x);
                _tweakPosition.x = Mathf.Clamp(offset, _anchors[lastIndex].x - margin, _anchors[0].x + margin);

                PlayTweaking(true);
            }
        }

        private void TouchTapHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(_self, args.position, sender.camera))
            {
                var count = Mathf.Min(_index + maximum, _rects.Count);
                for (var i = _index; i < count; ++i)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(_rects[i], args.position, sender.camera))
                    {
                        var eventArgs = new ListBoxEventArgs();
                        eventArgs.index = i;
                        OnSelectCallback(eventArgs);
                    }
                }
            }
        }        

        private IEnumerator Tweaking()
        {            
            while (_step < 1f)
            {
                _step = Mathf.Clamp01(_step + Time.deltaTime);
                container.anchoredPosition = Vector2.Lerp(container.anchoredPosition, _tweakPosition, _step);
                yield return null;
            }

            container.anchoredPosition = _tweakPosition;

            _tweaking = null;
        }        

        private IEnumerator Align()
        {
            var count = _items.Count;

            var queue = new int[count];
            for (var i = 0; i < count; ++i)
            {
                queue[i] = i;
            }

            var step = 0f;
            var loop = true;

            while (loop)
            {
                step = Mathf.Clamp01(step + Time.smoothDeltaTime);
                loop = false;

                for (var j = 0; j < count; ++j)
                {
                    if (queue[j] == -1)
                    {
                        continue;
                    }

                    var target = _points[j];
                    var rect = _rects[j];

                    rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, target, step);
                    if (Vector2.Distance(rect.anchoredPosition, target) < 2f)
                    {
                        rect.anchoredPosition = target;
                        queue[j] = -1;
                    }
                    else
                    {
                        loop = true;
                    }
                }

                yield return null;
            }

            _align = null;
        }
        #endregion
    }

}
