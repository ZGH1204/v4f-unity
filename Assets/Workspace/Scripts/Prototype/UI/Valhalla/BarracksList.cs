// <copyright file="ValhallaList.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

using V4F.Game;

namespace V4F.Prototype.UI.Valhalla
{

    public class BarracksList : ListBox<ValhallaItem>
    {
        public ValhallaItem itemPrefab;

        protected override ValhallaItem prefab
        {
            get { return itemPrefab; }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            var heroes = Database.QueryHeroesByLocation(Location.Reserve);
            for (var i = 0; i < heroes.Length; ++i)
            {
                var item = AddItem();
                item.actor = heroes[i];
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Clear();
        }
    }

}
