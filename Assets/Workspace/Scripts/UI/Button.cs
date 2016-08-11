// <copyright file="Button.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEngine.UI;

namespace V4F.UI
{

    public class Button : MonoBehaviour
    {
        public delegate void ButtonEventHandler(Button sender, ButtonEventArgs args);

        public Sprite normal;
        public Sprite pressed;
        public Sprite disabled;

        public event ButtonEventHandler OnClick;

        private RectTransform _self;
        private Image _image;

        private bool _disable;
        private bool _capture;

        public bool disable
        {
            get { return _disable; }
            set
            {
                _disable = value;
                _image.sprite = ((!_disable) ? normal : disabled);
            }
        }

        private void TouchDownHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (!_disable)
            {
                _capture = RectTransformUtility.RectangleContainsScreenPoint(_self, args.position, sender.camera);
                if (_capture)
                {
                    _image.sprite = pressed;
                }                
            }
        }

        private void TouchUpHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (_capture)
            {
                _image.sprite = normal;
                _capture = false;
            }
        }

        private void TouchTapHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (!_disable && _capture)
            {
                OnClickCallback(null);                
            }
        }

        private void OnClickCallback(ButtonEventArgs args)
        {
            if (OnClick != null)
            {
                OnClick(this, args);
            }
        }

        private void Awake()
        {
            _self = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            TouchAdapter.OnTouchDown += TouchDownHandler;
            TouchAdapter.OnTouchUp += TouchUpHandler;
            TouchAdapter.OnTouchTap += TouchTapHandler;
        }

        private void OnDisable()
        {
            TouchAdapter.OnTouchDown -= TouchDownHandler;
            TouchAdapter.OnTouchUp -= TouchUpHandler;
            TouchAdapter.OnTouchTap -= TouchTapHandler;
        }

    }
	
}
