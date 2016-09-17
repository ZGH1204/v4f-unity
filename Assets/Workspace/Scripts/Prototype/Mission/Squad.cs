// <copyright file="Squad.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using V4F.Character;

namespace V4F.Prototype.Mission
{

    public class Squad : Group
    {        
        private List<Monster> _monsters;

        public override bool isAlive
        {
            get { return (_monsters.Count > 0); }
        }

        public override int count
        {
            get { return _monsters.Count; }
        }

        public override Actor this[int index]
        {
            get { return _monsters[index]; }
        }

        public override void PlayTweaking(GroupCallback cb)
        {
            StartCoroutine(Tweaking(cb));
        }

        public override bool Change(Actor a, Actor b)
        {
            return false;
        }

        public void Prepare(int count)
        {
            _monsters = new List<Monster>(count);
        }

        public bool Spawn(Actor actor)
        {
            var index = _monsters.Count + 1;
            if (index == Mathf.Clamp(index, 1, transform.childCount))
            {
                var monster = new Monster(actor.puppet);
                monster.OnDeath += DeathMonsterHandler;
                monster.controlAI = true;

                var point = transform.GetChild(index - 1);

                monster.gameObject = Instantiate(monster.puppet.prefab, Vector3.zero, Quaternion.identity) as GameObject;
                monster.transform.localScale = Vector3.one;
                monster.transform.SetParent(point, false);                

                _monsters.Add(monster);

                return true;
            }
            
            return false;            
        }        

        private void DeathMonsterHandler(Monster sender)
        {
            sender.OnDeath -= DeathMonsterHandler;
            _monsters.Remove(sender);
        }

        private IEnumerator Tweaking(GroupCallback callback)
        {
            var count = _monsters.Count;
            var list = new List<Transform>(count);
            
            for (var i = 0; i < count; ++i)
            {
                var monster = _monsters[i];
                var point = transform.GetChild(i);

                var diff = monster.transform.position.x - point.position.x;
                if (diff > Mathf.Epsilon)
                {
                    monster.transform.SetParent(point, true);
                    list.Add(monster.transform);
                }
            }

            var origin = Vector3.zero;
            while (list.Count > 0)
            {
                count = list.Count;
                for (var i = 0; i < count; ++i)
                {
                    var point = list[i];
                    point.localPosition = Vector3.Lerp(point.localPosition, origin, Time.deltaTime * 2f);
                    if (Vector3.Distance(point.localPosition, origin) < 0.05f)
                    {
                        point.localPosition = origin;
                        list[i] = null;
                    }
                }

                list.RemoveAll(x => (x == null));

                yield return null;
            }

            list = null;

            if (callback != null)
            {
                callback(this);
            }
        }

    }
	
}
