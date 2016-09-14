// <copyright file="IState.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

namespace V4F.FSM
{    
    public interface IState
    {
        event StateEventHandler OnTransition;

        void EntryStart();

        void EntryEnd();

        IEnumerable Execute();

        void Exit();
    }
	
}
