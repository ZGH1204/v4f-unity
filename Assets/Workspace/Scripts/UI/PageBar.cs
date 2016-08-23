// <copyright file="PageBar.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace V4F.UI
{

    public class PageBar : MonoBehaviour
    {
        public delegate void PageBarEventHandler(PageBar sender, PageBarEventArgs args);

        public event PageBarEventHandler OnClick;

        public Sprite positive;
        public Sprite negative;

        private RectTransform _rect;
        private List<RectTransform> _knobRects;
        private List<Image> _knobs;
        private int _selectIndex;

        public void Select(int index)
        {
            if (_selectIndex != index)
            {
                if (_selectIndex != -1)
                {
                    var image = _knobs[_selectIndex];
                    image.sprite = negative;
                    _selectIndex = -1;
                }

                if (!(index < 0) && (index < _knobs.Count))
                {
                    var image = _knobs[index];
                    image.sprite = positive;
                    _selectIndex = index;
                }
            }                            
        }

        protected virtual void Awake()
        {
            _rect = GetComponent<RectTransform>();            

            var count = transform.childCount;
            _knobRects = new List<RectTransform>(count);
            _knobs = new List<Image>(count);
            for (var i = 0; i < count; ++i)
            {
                var child = transform.GetChild(i);
                var image = child.GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = negative;

                    _knobRects.Add(child.GetComponent<RectTransform>());
                    _knobs.Add(image);                    
                }
            }

            _selectIndex = -1;
        }

        protected virtual void OnEnable()
        {
            TouchAdapter.OnTouchTap += TouchTapHandler;
        }

        protected virtual void OnDisable()
        {
            TouchAdapter.OnTouchTap -= TouchTapHandler;
        }

        private void OnClickCallback(PageBarEventArgs args)
        {
            if (OnClick != null)
            {
                OnClick(this, args);
            }
        }

        private void TouchTapHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(_rect, args.position, sender.camera))
            {
                var count = _knobs.Count;                
                for (var i = 0; i < count; ++i)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(_knobRects[i], args.position, sender.camera))
                    {
                        Select(i);

                        var eventArgs = new PageBarEventArgs();
                        eventArgs.index = i;
                        OnClickCallback(eventArgs);

                        break;
                    }
                }
            }
        }
    }
	
}
