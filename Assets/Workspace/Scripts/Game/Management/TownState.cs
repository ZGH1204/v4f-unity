// <copyright file="TownState.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

using V4F.FSM;

namespace V4F.Game.Management
{

    public class TownState : State
    {        
        public TouchAdapter input;
        public GameObject town;

        private RaycastHit[] _hitResults = new RaycastHit[8];
        private int _layerMask = 0;

        public override void EntryStart()
        {
            town.SetActive(true);
        }

        public override void EntryEnd()
        {            
            input.OnTouchTap += TouchTapHandler;
            _layerMask = LayerMask.GetMask(new string[] { typeof(TownItem).Name });
        }

        public override void Exit()
        {
            input.OnTouchTap -= TouchTapHandler;
            _layerMask = 0;
        }

        public override IEnumerable Execute()
        {            
            while (true)
            {
                yield return null;
            }
        }

        private void TouchTapHandler(TouchAdapter sender, TouchEventArgs args)
        {
            var ray = sender.camera.ScreenPointToRay(args.position);
            var count = Physics.RaycastNonAlloc(ray, _hitResults, sender.camera.farClipPlane, _layerMask);
            if (count > 0)
            {
                var item = _hitResults[0].collider.GetComponent<TownItem>();
                if ((item != null) && item.unlocked)
                {
                    item.OnClick(this);
                }
            }
        }
    }
	
}
