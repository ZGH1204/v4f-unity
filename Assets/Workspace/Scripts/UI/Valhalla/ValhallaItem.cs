// <copyright file="ValhallaItem.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEngine.UI;

using V4F.Character;
using V4F.UI;

namespace V4F.UI.Valhalla
{

    public class ValhallaItem : MonoBehaviour, IListItem
    {        
        public RectTransform back;
        public RectTransform icon;
        public RectTransform front;        
        public RectTransform frame;        

        [Range(0f, 64f)]
        public float expFront = 12f;

        [Range(0f, 64f)]
        public float expFrame = 12f;

        private RectTransform _rect;
        private Image _iconImage;
        private Image _frameImage;
        private Actor _actor;        
        private bool _selected;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();

            _iconImage = icon.GetComponent<Image>();
            _frameImage = frame.GetComponent<Image>();
            _frameImage.enabled = false;

            _actor = null;
            _selected = false;
        }

        public RectTransform rect
        {
            get { return _rect; }
        }

        public Vector2 size
        {
            get { return _rect.sizeDelta; }
            set
            {
                back.sizeDelta = value;
                icon.sizeDelta = value;
                front.sizeDelta = new Vector2(value.x + expFront, value.y + expFront);
                frame.sizeDelta = new Vector2(value.x + expFrame, value.y + expFrame);

                _rect.sizeDelta = value;
            }
        }        

        public bool select
        {
            get { return _selected; }
            set
            {
                _selected = value;
                _frameImage.enabled = _selected;
            }
        }

        public Actor actor
        {
            get { return _actor; }
            set
            {
                _actor = value;
                _iconImage.sprite = _actor.puppet.icon;
            }
        }

        public void Clear()
        {
            _actor = null;
            _iconImage.sprite = null;
            _frameImage.enabled = false;
            _selected = false;
        }
    }
	
}
