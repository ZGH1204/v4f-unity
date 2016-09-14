// <copyright file="TestState01.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

using V4F.FSM;

namespace V4F.Game
{

    public class TestState01 : State
    {
        public StateTransition transition;
        public State next;

        public override void EntryStart()
        {
            gameObject.SetActive(true);
        }

        public override void Exit()
        {
            gameObject.SetActive(false);
        }

        public override IEnumerable Execute()
        {
            while (true)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    var args = new StateEventArgs(next, transition);
                    OnTransitionCallback(args);
                }
                yield return null;
            }            
        }
    }
	
}
