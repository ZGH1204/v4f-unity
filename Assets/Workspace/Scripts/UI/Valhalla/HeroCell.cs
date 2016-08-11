// <copyright file="Hero.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEngine.UI;

using V4F.Game;

namespace V4F.UI.Valhalla
{

    public class HeroCell : MonoBehaviour, ICell<Hero>, IDraggable
    {
        #region Fields
        private RectTransform _rect = null;        
        private Image _image = null;
        private Image _frame = null;
        private Hero _hero = null;
        private bool _select = false;
        #endregion

        #region Properties
        public RectTransform rect
        {
            get { return _rect; }
        }

        public Hero subject
        {
            get { return _hero; }
            set
            {
                _hero = value;
                _image.sprite = (!empty ? icon : null);
            }
        }

        public bool empty
        {
            get { return (_hero == null); }
        }

        public DraggableType type
        {
            get { return DraggableType.Hero; }
        }

        public Sprite icon
        {
            get { return _hero.puppet.icon; }
        }

        public bool focus
        {
            get { return _select; }
            set
            {
                _select = value;
                if (_frame != null)
                {
                    _frame.enabled = _select;
                }
            }
        }
        #endregion

        #region Methods
        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
            _select = false;

            var child = _rect.GetChild(0);
            if (child != null)
            {
                _frame = child.GetComponent<Image>();
                _frame.enabled = _select;
            }
        }
        #endregion
    }

}
