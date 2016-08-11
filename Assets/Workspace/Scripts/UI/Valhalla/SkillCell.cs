// <copyright file="Hero.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEngine.UI;

using V4F.Puppets;

namespace V4F.UI.Valhalla
{

    public class SkillCell : MonoBehaviour
    {
        #region Fields
        private RectTransform _self = null;
        private Image _image = null;
        private Image _frame = null;
        private PuppetSkill _skill = null;
        #endregion

        #region Properties
        public RectTransform rect
        {
            get { return _self; }
        }

        public PuppetSkill skill
        {
            get { return _skill; }
            set
            {
                _skill = value;

                _image.sprite = ((!empty) ? _skill.icon : null);
                _image.enabled = !empty;

                if (_frame != null)
                {
                    _frame.enabled = false;
                }                
            }
        }

        public bool empty
        {
            get { return _skill == null; }
        }

        public bool focus
        {
            set
            {
                if (!empty && _frame != null)
                {
                    _frame.enabled = value;
                }
            }
        }
        #endregion

        #region Methods
        private void Awake()
        {
            _self = GetComponent<RectTransform>();
            _image = GetComponent<Image>();

            var child = _self.GetChild(0);
            if (child != null)
            {
                _frame = child.GetComponent<Image>();
                _frame.enabled = false;
            }
        }
        #endregion
    }

}
