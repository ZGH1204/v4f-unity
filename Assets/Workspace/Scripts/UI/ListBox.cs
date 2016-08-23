// <copyright file="ListBox.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace V4F.UI
{

    public abstract class ListBox<T> : MonoBehaviour where T : MonoBehaviour, IListItem
    {
        #region Types
        public delegate void ListBoxEventHandler(ListBox<T> sender, ListBoxEventArgs args);
        #endregion

        #region Events
        public event ListBoxEventHandler OnSelect;
        public event ListBoxEventHandler OnRemove;
        #endregion

        #region Fields
        public Vector2 sizeCell;
        public ListBoxDirection direction = ListBoxDirection.LeftRight;
        public float margin;
        public float divider;
        public int availableCount = 4;

        private RectTransform _rect;
        private RectTransform _rectAnchor;
        private RectTransform _rectPool;
        private List<Vector2> _anchors;    
        private List<T> _items;
        private List<Vector2> _points;
        private Vector2 _anchorPosition;
        private Vector2 _tweakPosition;
        private Vector2 _capturePosition;
        private Vector2 _afterPosition;
        private Vector2 _beforPosition;        
        private int _anchorIndex;
        private int _selectIndex;
        private bool _captureScroll;

        private IEnumerator _tweaking = null;
        private IEnumerator _align = null;
        private float _step = 0f;
        #endregion

        #region Properties
        protected abstract T prefab { get; }

        public T this[int index]
        {
            get { return _items[index]; }
        }
        #endregion

        #region Methods
        public T AddItem()
        {
            var index = _items.Count;

            var cell = GetFreeCell();
            cell.SetActive(true);

            var cellPosition = direction.GetFirstPosition(sizeCell, margin) + direction.GetOffset(sizeCell, divider) * index;
            _points.Add(cellPosition);

            var anchorPosition = ((_anchors.Count > 0) ? _anchors[_anchors.Count - 1] : _anchorPosition);
            if (!(index < availableCount))
            {
                anchorPosition += direction.GetAnchorOffset(sizeCell, divider);
            }
            _anchors.Add(anchorPosition);

            var rect = cell.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.SetParent(_rectAnchor, false);
            rect.anchoredPosition = cellPosition;            

            var item = cell.GetComponent<T>();
            item.size = sizeCell;
            _items.Add(item);

            return item;
        }

        public void Clear()
        {
            var count = _items.Count;
            for (var i = 0; i < count; ++i)
            {
                var item = _items[i];
                item.Clear();

                item.gameObject.SetActive(false);
                var form = item.gameObject.transform;
                form.SetParent(_rectPool, false);
                form.SetAsFirstSibling();
            }

            _selectIndex = -1;

            _anchors.Clear();
            _points.Clear();
            _items.Clear();
        }

        public void ResetSelection()
        {
            if (_selectIndex != -1)
            {
                _items[_selectIndex].select = false;
                _selectIndex = -1;
            }
        }

        public void RemoveAt(int index)
        {
            var args = new ListBoxEventArgs();
            args.index = index;
            OnRemoveCallback(args);

            var item = _items[index];

            if ((_selectIndex == index) || item.select)
            {
                item.select = false;
                _selectIndex = -1;
            }            

            var last = _points[index];
            _points.RemoveAt(index);
            for (var i = index; i < _points.Count; ++i)
            {
                var next = _points[i];
                _points[i] = last;
                last = next;
            }
            
            _items.RemoveAt(index);

            item.gameObject.SetActive(false);
            var form = item.gameObject.transform;
            form.SetParent(_rectPool, false);
            form.SetAsFirstSibling();

            PlayAlign();            
        }

        protected virtual void Awake()
        {
            _rect = GetComponent<RectTransform>();            
            
            _rectAnchor = new GameObject("anchor").AddComponent<RectTransform>();
            _rectAnchor.SetParent(_rect, false);
            direction.SetupAnchor(ref _rectAnchor);

            _rectPool = new GameObject("pool").AddComponent<RectTransform>();
            _rectPool.SetParent(_rect, false);
            direction.SetupAnchor(ref _rectPool);

            _anchors = new List<Vector2>(64);
            _anchorPosition = _rectAnchor.anchoredPosition;

            _items = new List<T>(64);

            _points = new List<Vector2>(64);

            _anchorIndex = 0;
            _selectIndex = -1;
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

        private GameObject GetFreeCell()
        {
            GameObject cell = null;

            var rect = ((_rectPool.childCount > 0) ? _rectPool.GetChild(0) : null);
            if (rect != null)
            {
                cell = rect.gameObject;
            }
            else
            {
                cell = Instantiate(prefab.gameObject);
            }

            return cell;
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
            _captureScroll = RectTransformUtility.RectangleContainsScreenPoint(_rect, args.position, sender.camera);
            if (_captureScroll)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, args.position, sender.camera, out _beforPosition);
                _capturePosition = _rectAnchor.anchoredPosition;
                StopTweaking();
            }            
        }

        private void TouchUpHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (_captureScroll)
            {                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, args.position, sender.camera, out _afterPosition);
                var target = _capturePosition + direction.DeltaTransform(_beforPosition, _afterPosition);
                var index = direction.GetClosestAnchor(_anchors, target, out target);
                PlayTweaking(target, false, true);
                
                _anchorIndex = Mathf.Clamp(index - (availableCount - 1), 0, Mathf.Max(0, _items.Count - availableCount));
                _captureScroll = false;                
            }            
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
            if (_captureScroll)
            {
                var count = Mathf.Min(_anchorIndex + availableCount, _items.Count);
                for (var i = _anchorIndex; i < count; ++i)
                {                    
                    if (RectTransformUtility.RectangleContainsScreenPoint(_items[i].rect, args.position, sender.camera))
                    {
                        if (_selectIndex != -1)
                        {
                            _items[_selectIndex].select = false;
                        }                
                                
                        _items[i].select = true;
                        _selectIndex = i;

                        var eventArgs = new ListBoxEventArgs();
                        eventArgs.index = _selectIndex;
                        OnSelectCallback(eventArgs);

                        break;
                    }
                }
            }
        }

        private void PlayTweaking(Vector2 target, bool inMoving, bool closest)
        {
            var tweaking = (closest ? 0f : divider * 1.5f);
            if (!inMoving)
            {
                _step = 0f;
            }

            _tweakPosition = direction.ClampTransform(target, _anchors[0], _anchors[_anchors.Count - 1], tweaking);

            if (_tweaking == null)
            {
                _tweaking = Tweaking(_rectAnchor);
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
                    var rect = _items[j].rect;

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
