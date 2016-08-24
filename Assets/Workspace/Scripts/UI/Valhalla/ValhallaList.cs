// <copyright file="ValhallaList.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

using V4F.Game;
using V4F.UI;

namespace V4F.UI.Valhalla
{

    public class ValhallaList : ListBox<ValhallaItem>
    {
        public ValhallaItem itemPrefab;

        protected override ValhallaItem prefab
        {
            get { return itemPrefab; }            
        }

        protected override void OnEnable()
        {
            ValhallaItem item = null;

            base.OnEnable();
            
            var heroes = Database.QueryHeroesByLocation(Location.Valhalla);
            for (var i = 0; i < heroes.Length; ++i)
            {
                AddItem(out item);
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
