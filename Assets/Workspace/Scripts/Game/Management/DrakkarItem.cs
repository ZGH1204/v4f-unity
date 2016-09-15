// <copyright file="DrakkarItem.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

using V4F.FSM;
using V4F.Character;

namespace V4F.Game.Management
{

    public class DrakkarItem : TownItem
    {
        [SerializeField, HideInInspector]
        private string _scenePath = null;

        [SerializeField, HideInInspector]
        private string _sceneName = null;

        public string scenePath
        {
            get { return _scenePath; }
        }

        public string sceneName
        {
            get { return _sceneName; }
        }

        public override void OnClick(State current)
        {
            var actors = Database.QueryHeroesByLocation(Location.Reserve);
            if (actors.Length > 0)
            {
                var args = new StateEventArgs(current, transition);
                current.OnTransitionCallback(args);
            }            
        }
    }

}
