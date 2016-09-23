// <copyright file="SkillItem.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEngine.UI;

using V4F.Character;

namespace V4F.UI.Valhalla
{

    public class SkillItem : MonoBehaviour, IListItem
    {
        public RectTransform back;
        public RectTransform icon;        
        public RectTransform frame;        

        [Range(0f, 64f)]
        public float expFrame = 12f;
        
        private Image _iconImage;
        private Image _frameImage;
        private Skill _skill;
        private bool _selected;

        private void Awake()
        {            
            _iconImage = icon.GetComponent<Image>();
            _frameImage = frame.GetComponent<Image>();
            _frameImage.enabled = false;

            _skill = null;
            _selected = false;
        }

        public RectTransform rect
        {
            get { return back; }
        }

        public Vector2 size
        {
            get { return back.sizeDelta; }
            set
            {
                back.sizeDelta = value;
                icon.sizeDelta = value;
                frame.sizeDelta = new Vector2(value.x + expFrame, value.y + expFrame);                
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

        public Skill skill
        {
            get { return _skill; }
            set
            {
                _skill = value;
                _iconImage.sprite = _skill[0].icon;
            }
        }

        public void Clear()
        {
            _skill = null;
            _iconImage.sprite = null;
            _frameImage.enabled = false;
            _selected = false;
        }
    }

}
