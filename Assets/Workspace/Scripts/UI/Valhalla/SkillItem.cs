// <copyright file="SkillItem.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEngine.UI;

using V4F.Character;

namespace V4F.UI.Valhalla
{

    public class SkillItem : MonoBehaviour, IListItem
    {
        private RectTransform _rect;
        private Image _rectImage;
        private RectTransform _frame;
        private Image _frameImage;
        private Skill _skill;
        private bool _selected;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _rectImage = _rect.GetComponent<Image>();
            _frame = _rect.GetChild(0).GetComponent<RectTransform>();
            _frameImage = _frame.GetComponent<Image>();
            _frameImage.enabled = false;

            _skill = null;
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
                _rect.sizeDelta = value;
                _frame.sizeDelta = value;
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
                _rectImage.sprite = _skill[0].icon;
            }
        }

        public void Clear()
        {
            _skill = null;
            _rectImage.sprite = null;
            _frameImage.enabled = false;
            _selected = false;
        }
    }

}
