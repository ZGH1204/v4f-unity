// <copyright file="Controller.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

using V4F.FSM;
using V4F.UI.Map;

namespace V4F.Prototype.Mission
{

    public class Map : State
    {
        public MapHandler mapHandler;
        public StateTransition transition;
        public State next;        

        public override void EntryStart()
        {
            mapHandler.OnClosed += MapClosedHandler;
            mapHandler.gameObject.SetActive(true);
        }

        public override void Exit()
        {
            mapHandler.OnClosed -= MapClosedHandler;
            mapHandler.gameObject.SetActive(false);
        }

        public override IEnumerable Execute()
        {
            while (true)
            {
                yield return null;
            }            
        }

        private void MapClosedHandler(MapHandler sender, MapEventArgs args)
        {
            var eventArgs = new StateEventArgs(next, transition);
            OnTransitionCallback(eventArgs);
        }
    }
	
}
