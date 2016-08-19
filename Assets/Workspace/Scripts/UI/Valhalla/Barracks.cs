// <copyright file="Barracks.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

using V4F.Game;

namespace V4F.UI.Valhalla
{

    public class Barracks : ListBox<ActorCell>
    {
        #region Fields
        public RectTransform content;
        public GameObject prefabCell;
        public float marginValue;
        public float intervalValue;        
        public int maximumValue;

        private int _selected;
        #endregion

        #region Properties
        protected override GameObject prefab
        {
            get { return prefabCell; }
        }

        protected override RectTransform container
        {
            get { return content; }
        }

        protected override float margin
        {
            get { return marginValue; }
        }

        protected override float interval
        {
            get { return intervalValue; }
        }

        protected override int maximum
        {
            get { return maximumValue; }
        }
        #endregion

        #region Methods
        protected override void Awake()
        {
            base.Awake();
            _selected = -1;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            OnSelect += OnSelectHandler;
            OnRemove += OnRemoveHandler;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnSelect -= OnSelectHandler;
            OnRemove -= OnRemoveHandler;
        }

        private void OnSelectHandler(ListBox<ActorCell> sender, ListBoxEventArgs args)
        {
            if (_selected != args.index)
            {
                if (_selected != -1)
                {
                    sender[_selected].focus = false;
                }

                _selected = args.index;
                sender[_selected].focus = true;
            }
            else if (_selected != -1)
            {
                var cell = sender[_selected];
                if (!cell.focus)
                {
                    cell.focus = true;
                }
            }
        }

        private void OnRemoveHandler(ListBox<ActorCell> sender, ListBoxEventArgs args)
        {
            if (_selected == args.index)
            {
                _selected = -1;
            }
        }

        private void Start()
        {
            var heroes = Database.QueryHeroesByLocation(Location.Reserve);
            for (var i = 0; i < heroes.Length; ++i)
            {
                var item = Add();
                item.subject = heroes[i];
            }
        }
        #endregion
    }

}
