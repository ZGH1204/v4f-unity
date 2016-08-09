// <copyright file="PuppetItem.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEngine.UI;

using V4F.Game;

namespace V4F.UI.Draft
{

    [DisallowMultipleComponent]
    public class HeroItem : MonoBehaviour, IDraggable, ICell<Hero>
    {
        #region Fields
        private RectTransform _transform;
        private Image _image;
        private Hero _hero;
        #endregion

        #region Properties
        public DraggableType type
        {
            get { return DraggableType.Hero; }
        }

        public Sprite icon
        {
            get { return _hero.puppet.icon; }
        }        

        public Hero subject
        {
            get { return _hero; }

            set
            {
                if (value != null)
                {
                    _image.sprite = value.puppet.icon;
                }
                else
                {
                    _image.sprite = null;
                }

                _hero = value;
            }
        }

        public bool empty
        {
            get { return false; }
        }

        public RectTransform rect
        {
            get { return _transform; }
        }
        #endregion

        #region Methods
        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
        }
        #endregion
    }

}
