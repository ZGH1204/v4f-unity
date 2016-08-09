// <copyright file="Heroes.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

using V4F.Game;

namespace V4F.UI.Draft
{

    [DisallowMultipleComponent]
    public class Heroes : ScrollList<HeroItem>
    {
        #region Fields
        public DraggableSubject subject;
        public DraggableSubject subject2;
        #endregion

        #region Methods        
        protected override bool OnCaptureItem(int index, Vector2 screenPoint, Camera camera)
        {
            return false;
        }

        private void OnDrop(DraggableSubject sender, DraggableEventArgs args)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(transform, args.point, args.camera))
            {
                var item = AddItem();
                item.subject = null;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            subject2.OnDrop += OnDrop;
        }

        protected override void OnDisable()
        {
            base.OnEnable();
            subject2.OnDrop -= OnDrop;
        }
        #endregion
    }

}
