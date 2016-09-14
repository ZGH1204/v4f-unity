// <copyright file="StateEventArgs.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System;

namespace V4F.FSM
{
    public delegate void StateEventHandler(IState sender, StateEventArgs args);

    public class StateEventArgs : EventArgs
    {        
        public IState nextState
        {
            get;
            private set;
        }

        public IStateTransition transition
        {
            get;
            private set;
        }

        public StateEventArgs(IState nextState, IStateTransition transition)
        {
            this.nextState = nextState;
            this.transition = transition;
        }
    }
	
}
