// <copyright file="Button.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEngine.UI;

namespace V4F.UI
{

    public class Button : MonoBehaviour
    {
        #region Types
        public delegate void ButtonEventHandler(Button sender, ButtonEventArgs args);
        #endregion

        #region Events
        public event ButtonEventHandler OnClick;
        #endregion

        #region Fields
        public Sprite normal;
        public Sprite pressed;
        public Sprite disabled;

        private RectTransform _rect;
        private Image _image;

        private bool _clickEvent;
        private bool _disable;
        private bool _capture;
        private bool _locked;        
        #endregion

        #region Properties
        public bool disable
        {
            get { return _disable; }
            set
            {
                _image.sprite = ((value) ? disabled : normal);
                _disable = value;
            }
        }

        public bool locked
        {
            get { return _locked; }
            set { _locked = value; }
        }

        public RectTransform rect
        {
            get { return _rect; }
        }

        protected Image image
        {
            get { return _image; }
        }
        #endregion

        #region Methods
        protected virtual void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
        }

        protected virtual void OnEnable()
        {
            TouchAdapter.OnTouchDown += TouchDownHandler;
            TouchAdapter.OnTouchUp += TouchUpHandler;
            TouchAdapter.OnTouchTap += TouchTapHandler;
        }

        protected virtual void OnDisable()
        {
            TouchAdapter.OnTouchDown -= TouchDownHandler;
            TouchAdapter.OnTouchUp -= TouchUpHandler;
            TouchAdapter.OnTouchTap -= TouchTapHandler;
        }

        protected virtual void Start()
        {
            _image.sprite = normal;
            _clickEvent = false;
            _disable = false;
            _capture = false;
            _locked = false;
        }

        private void OnClickCallback(ButtonEventArgs args)
        {
            if (OnClick != null)
            {
                OnClick(this, args);
            }
        }

        private void TouchDownHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (!(_locked || _disable))
            {
                _capture = RectTransformUtility.RectangleContainsScreenPoint(_rect, args.position, sender.camera);
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

                if (_clickEvent || RectTransformUtility.RectangleContainsScreenPoint(_rect, args.position, sender.camera))
                {
                    var eventArgs = new ButtonEventArgs();
                    OnClickCallback(eventArgs);

                    _clickEvent = false;
                }
            }
        }

        private void TouchTapHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (_capture)
            {
                _clickEvent = true;                
            }
        }
        #endregion
    }

}
