// <copyright file="Loading.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

using V4F.FSM;
using V4F.Prototype.Map;
using V4F.Character;
using V4F.Game;

namespace V4F.Prototype.Mission
{

    public class Loading : State
    {
        public delegate void BeginHandler(int length, int width);
        public delegate void EndHandler(int entry);
        public delegate void NodeHandler(Node node);

        public static event BeginHandler OnBegin;
        public static event EndHandler OnEnd;
        public static event NodeHandler OnNode;

        public TouchAdapter input;
        public Data mapData;

        public GameObject textLoading;
        public GameObject textContinue;
        public GameObject partyController;

        public Party party;        

        public StateTransition transition;
        public State next;

        private bool _waitingTap;

        private static void OnBeginCallback(Data data)
        {
            if (OnBegin != null)
            {
                OnBegin(data.length, data.width);
            }
        }

        private static void OnEndCallback(Data data)
        {
            if (OnEnd != null)
            {
                OnEnd(data.entry);
            }
        }

        private static void OnNodeCallback(Node node)
        {
            if (OnNode != null)
            {
                OnNode(node);
            }
        }

        public override void EntryStart()
        {
            textContinue.SetActive(false);
            textLoading.SetActive(true);
            gameObject.SetActive(true);
        }

        public override void Exit()
        {
            gameObject.SetActive(false);
            partyController.SetActive(true);
        }

        public override IEnumerable Execute()
        {
            OnBeginCallback(mapData);
            yield return StartCoroutine(Procces());
            OnEndCallback(mapData);

            textLoading.SetActive(false);
            textContinue.SetActive(true);
            
            input.OnTouchTap += TouchTapHandler;

            _waitingTap = true;
            while (_waitingTap)
            {                                
                yield return null;
            }

            input.OnTouchTap -= TouchTapHandler;

            var args = new StateEventArgs(next, transition);
            OnTransitionCallback(args);
        }

        private IEnumerator Procces()
        {
            var counter = 0;
            for (var i = 0; i < mapData.length; ++i)
            {
                OnNodeCallback(mapData[i]);
                if (++counter > 16)
                {
                    yield return null;
                    counter = 0;
                }
            }

            var heroes = Database.QueryHeroesByLocation(Game.Location.Reserve);
            var count = Mathf.Min(heroes.Length, 4);

            party.Prepare(count);
            for (var i = 0; (i < count) && party.Enter(heroes[i]); ++i);
        }

        private void TouchTapHandler(TouchAdapter sender, TouchEventArgs args)
        {
            _waitingTap = false;
        }

    }
	
}
