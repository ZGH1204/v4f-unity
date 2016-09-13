// <copyright file="StateMachine.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

namespace V4F.FSM
{

    public class StateMachine : MonoBehaviour
    {
        #region Fields
        public State originalState;

        private IState _current;
        private IState _next;
        private IStateTransition _transition;
        #endregion        

        #region Methods
        private IEnumerable Execute()
        {
            while (true)
            {
                for(var e = _current.Execute().GetEnumerator(); (_transition == null) && e.MoveNext();)
                {
                    yield return e.Current;
                }

                while (_transition.Waiting())
                {
                    yield return null;
                }

                _current.OnTransition -= StateTransitionHandler;                

                foreach (var e in _transition.ExitPrevState())
                {
                    yield return e;
                }                

                _current.Exit();
                _current = _next;

                if (_current.Exists())
                {                    
                    _current.OnTransition += StateTransitionHandler;
                    _current.EntryStart();                                        

                    foreach (var e in _transition.EnterNextState())
                    {
                        yield return e;
                    }

                    _transition = null;
                    _next = null;

                    _current.EntryEnd();
                }
                else
                {
                    _transition = null;

                    break;
                }                
            }

            gameObject.SetActive(false);
        }

        private void StateTransitionHandler(IState sender, StateEventArgs args)
        {
            _next = args.nextState;
            _transition = args.transition;
        }

        private void Start()
        {
            _current = originalState;
            _current.OnTransition += StateTransitionHandler;
            _current.EntryStart();
            _current.EntryEnd();

            StartCoroutine(Execute().GetEnumerator());
        }
        #endregion
    }

}
