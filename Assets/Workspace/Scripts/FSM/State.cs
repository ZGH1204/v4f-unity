// <copyright file="State.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

namespace V4F.FSM
{

    public class State : MonoBehaviour, IState
    {
        public event StateEventHandler OnTransition;

        public virtual void EntryStart()
        {

        }

        public virtual void EntryEnd()
        {
            
        }        

        public virtual IEnumerable Execute()
        {
            yield return null;
        }

        public virtual void Exit()
        {
            
        }

        protected void OnTransitionCallback(StateEventArgs args)
        {
            if (OnTransition != null)
            {
                OnTransition(this, args);
            }
        }
    }

}
