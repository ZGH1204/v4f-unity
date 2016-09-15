// <copyright file="TownItem.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

using V4F.FSM;

namespace V4F.Game.Management
{

    public class TownItem : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private StateTransition _transition= null;

        [SerializeField, HideInInspector]
        private State _state = null;

        [SerializeField, HideInInspector]
        private bool _unlocked = true;

        public StateTransition transition
        {
            get { return _transition; }
        }

        public State state
        {
            get { return _state; }
        }

        public bool unlocked
        {
            get { return _unlocked; }
        }

        public virtual void OnClick(State current)
        {
            var args = new StateEventArgs(_state, _transition);
            current.OnTransitionCallback(args);
        }
    }
	
}
