// <copyright file="Traveling.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

using V4F.FSM;
using V4F.UI.Mission;
using V4F.Prototype.Map;

namespace V4F.Prototype.Mission
{

    public class Traveling : State
    {
        public Location location;

        public Renderer room;

        public Renderer section1;
        public Renderer section2;
        public Renderer section3;
        public Renderer section4;

        public GameObject corridorObject;
        public GameObject roomObject;        

        public PartyController partyController;
        public PlayHandler playHandler;

        public StateTransition locationChangedTransition;

        public StateTransition showMapTransition;
        public State showMapState;

        public StateTransition combatTransition;
        public State combatState;

        private GameObject _currentObject = null;
        private bool _locationChanged = true;
        private bool _battleBegin = false;

        public override void EntryStart()
        {
            var node = location.current;
            var type = node.type;

            if (_locationChanged)
            {                
                if (type == NodeType.Room)
                {
                    var content = location.currentContent;
                    room.sharedMaterial = location.GetMaterial(type, content[0]);

                    _currentObject = roomObject;
                }
                else
                {
                    var content = location.currentContent;
                    section1.sharedMaterial = location.GetMaterial(type, content[0]);
                    section2.sharedMaterial = location.GetMaterial(type, content[1]);
                    section3.sharedMaterial = location.GetMaterial(type, content[2]);
                    section4.sharedMaterial = location.GetMaterial(type, content[3]);

                    _currentObject = corridorObject;
                }

                playHandler.Entry(type);
                _currentObject.SetActive(true);
                partyController.Entry(type);
            }

            if (_battleBegin)
            {
                playHandler.Entry(type);
                partyController.Resume();
            }
        }

        public override void EntryEnd()
        {
            _locationChanged = false;
            _battleBegin = false;
        }

        public override void Exit()
        {            
            if (_locationChanged || _battleBegin)
            {
                playHandler.Exit();
                partyController.Exit();
            }

            if (_locationChanged)
            {             
                if (_currentObject != null)
                {
                    _currentObject.SetActive(false);
                    _currentObject = null;
                }
            }
        }

        public override IEnumerable Execute()
        {
            while (true)
            {
                if (location.showMapTrigger)
                {                    
                    var args = new StateEventArgs(showMapState, showMapTransition);
                    OnTransitionCallback(args);
                }

                if (location.combatTrigger)
                {
                    _battleBegin = true;
                    var args = new StateEventArgs(combatState, combatTransition);
                    OnTransitionCallback(args);
                }

                if (location.changeRoomTrigger)
                {
                    _locationChanged = true;
                    var args = new StateEventArgs(this, locationChangedTransition);
                    OnTransitionCallback(args);
                }

                yield return null;
            }
        }        

    }
	
}
