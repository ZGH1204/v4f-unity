// <copyright file="Controller.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

using V4F.FSM;
using V4F.Prototype.Map;

namespace V4F.Prototype.Mission
{

    public class Gameplay : State
    {
        public Location location;

        public Renderer room;

        public Renderer section1;
        public Renderer section2;
        public Renderer section3;
        public Renderer section4;

        public GameObject corridorObject;
        public GameObject roomObject;

        public StateTransition locationChangedTransition;

        public StateTransition showMapTransition;
        public State showMapState;

        private GameObject _currentObject = null;
        private bool _locationChanged = true;

        public override void EntryStart()
        {
            if (_locationChanged)
            {
                var node = location.current;
                var type = node.type;

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

                _currentObject.SetActive(true);
            }            
        }

        public override void EntryEnd()
        {
            _locationChanged = false;
        }

        public override void Exit()
        {
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
                if (Input.GetKeyDown(KeyCode.M))
                {                    
                    var args = new StateEventArgs(showMapState, showMapTransition);
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
