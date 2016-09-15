// <copyright file="ValhallaState.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using V4F.FSM;

namespace V4F.Game.Management
{

    public class ValhallaState : State
    {
        public TouchAdapter input;

        public StateTransition transition = null;
        public State state = null;

        public override void EntryStart()
        {
         
        }

        public override void EntryEnd()
        {
            input.OnBack += BackHandler;
        }

        public override void Exit()
        {
            input.OnBack -= BackHandler;
        }

        public override IEnumerable Execute()
        {
            while (true)
            {
                yield return null;
            }
        }

        private void BackHandler(TouchAdapter sender)
        {
            var args = new StateEventArgs(state, transition);
            OnTransitionCallback(args);
        }
    }
	
}
