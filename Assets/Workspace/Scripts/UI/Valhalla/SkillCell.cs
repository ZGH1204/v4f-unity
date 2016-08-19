// <copyright file="Hero.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEngine.UI;

using V4F.Character;

namespace V4F.UI.Valhalla
{

    public class SkillCell : MonoBehaviour
    {
        #region Fields
        private RectTransform _self = null;
        private Image _image = null;
        private Image _frame = null;
        private Skill _skill = null;
        #endregion

        #region Properties
        public RectTransform rect
        {
            get { return _self; }
        }

        public Skill subject
        {
            get { return _skill; }
            set
            {
                _skill = value;

                _frame.enabled = false;
                _image.enabled = !empty;
                _image.sprite = _skill[0].icon;
            }
        }

        public bool empty
        {
            get { return _skill == null; }
        }

        public bool focus
        {
            set { _frame.enabled = (!empty && value); }
        }
        #endregion

        #region Methods
        private void Awake()
        {
            _self = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
            _frame = _self.GetChild(0).GetComponent<Image>();
        }
        #endregion
    }

}
