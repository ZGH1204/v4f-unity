// <copyright file="Battle.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

using V4F.FSM;
using V4F.UI.Mission;
using V4F.Prototype.Map;

namespace V4F.Prototype.Mission
{

    public class Battle : State
    {
        public delegate void BattleEventHandler();

        public event BattleEventHandler OnWin;
        public event BattleEventHandler OnLose;

        public StateTransition transition;
        public State state;

        public override void EntryStart()
        {


        }

        public override void EntryEnd()
        {
            Debug.Log("Battle");
        }

        public override void Exit()
        {

        }

        public override IEnumerable Execute()
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    OnWinCallback();

                    var args = new StateEventArgs(state, transition);
                    OnTransitionCallback(args);
                }

                yield return null;
            }
        }

        private void OnWinCallback()
        {
            if (OnWin != null)
            {
                OnWin();
            }
        }

        private void OnLoseCallback()
        {
            if (OnLose != null)
            {
                OnLose();
            }
        }
    }
    	
}
