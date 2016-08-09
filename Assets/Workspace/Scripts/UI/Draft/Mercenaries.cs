// <copyright file="Mercenaries.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

using V4F.Game;

namespace V4F.UI.Draft
{

    [DisallowMultipleComponent]
    public class Mercenaries : ScrollList<HeroItem>
    {
        #region Fields
        public DraggableSubject subject;

        private HeroItem _captureHero;
        private int      _captureIndex;
        #endregion

        #region Methods        
        protected override bool OnCaptureItem(int index, Vector2 screenPoint, Camera camera)
        {
            _captureHero = this[index];
            _captureIndex = index;

            Vector2 point = screenPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(subject.region, screenPoint, camera, out point))
            {
                subject.Attach(point, _captureHero);
                return true;
            }

            return false;
        }

        private void Start()
        {
            var heroes = Database.QueryHeroesByLocation(Location.Draft);
            for (var i = 0; i < heroes.Length; ++i)
            {
                var item = AddItem();
                item.subject = heroes[i];
            }            
        }
        #endregion
    }

}
