// <copyright file="DraggableSubject.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEngine.UI;

namespace V4F.UI
{

    [DisallowMultipleComponent]
    public class DraggableSubject : MonoBehaviour
    {
        public delegate void DraggableEventHandler(DraggableSubject sender, DraggableEventArgs args);

        #region Fields
        [Tooltip("Смещение активной точки контроля от якорной точки объекта")]
        public Vector3 offset = Vector3.zero;

        [Range(1f, 100f)]
        public float smoothness = 20f;

        private RectTransform _transform;
        private RectTransform _region;
        private Image _image;
        private IDraggable _draggable;
        #endregion

        #region Properties
        public RectTransform region
        {
            get { return _region; }
        }

        public event DraggableEventHandler OnDrag;
        public event DraggableEventHandler OnDrop;
        #endregion

        #region Methods
        public void Attach(Vector2 point, IDraggable draggable)
        {
            _transform.anchoredPosition = point;            
            _image.sprite = draggable.icon;
            _draggable = draggable;

            gameObject.SetActive(true);
        }

        private void OnDragCallback(DraggableEventArgs args)
        {
            if (OnDrag != null)
            {
                OnDrag(this, args);
            }
        }

        private void OnDropCallback(DraggableEventArgs args)
        {
            if (OnDrop != null)
            {
                OnDrop(this, args);
            }
        }

        private void TouchPressHandler(TouchAdapter sender, TouchEventArgs args)
        {
            var target = _transform.anchoredPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_region, args.position, sender.camera, out target))
            {
                _transform.anchoredPosition = Vector2.Lerp(_transform.anchoredPosition, target, smoothness * Time.deltaTime);

                var drag = new DraggableEventArgs();
                drag.point = args.position + offset;
                drag.type = _draggable.type;
                drag.camera = sender.camera;
                OnDragCallback(drag);
            }
        }

        private void TouchUpHandler(TouchAdapter sender, TouchEventArgs args)
        {
            var drop = new DraggableEventArgs();
            drop.point = args.position + offset;
            drop.type = _draggable.type;
            drop.camera = sender.camera;
            OnDropCallback(drop);

            _image.sprite = null;
            _draggable = null;

            gameObject.SetActive(false);
        }        

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
            _region = _transform.parent.GetComponent<RectTransform>();
            _image = GetComponent<Image>();

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            TouchAdapter.OnTouchPress += TouchPressHandler;
            TouchAdapter.OnTouchUp += TouchUpHandler;
        }

        private void OnDisable()
        {
            TouchAdapter.OnTouchPress -= TouchPressHandler;
            TouchAdapter.OnTouchUp -= TouchUpHandler;
        }
        #endregion
    }

}
