// <copyright file="Party.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using V4F.Character;

namespace V4F.Prototype.Mission
{

    public class Party : Group
    {        
        private List<Actor> _heroes;

        public override bool isAlive
        {
            get { return (_heroes.Count > 0); }
        }

        public override int count
        {
            get { return _heroes.Count; }
        }

        public override Actor this[int index]
        {
            get { return _heroes[index]; }
        }

        public override void PlayTweaking(GroupCallback cb)
        {
            StartCoroutine(Tweaking(cb));
        }

        public override bool Change(Actor a, Actor b)
        {
            var i1 = _heroes.IndexOf(a);
            var i2 = _heroes.IndexOf(b);

            if ((i1 != -1) && (i2 != -1))
            {
                _heroes[i1] = b;
                _heroes[i2] = a;

                return true;
            }

            return false;
        }

        public void Prepare(int count)
        {
            _heroes = new List<Actor>(count);
        }

        public bool Enter(Actor actor)
        {
            var index = _heroes.Count + 1;
            if (index == Mathf.Clamp(index, 1, transform.childCount))
            {
                var hero = new Hero(actor);
                hero.OnDeath += DeathHeroHandler;
                hero.controlAI = false;

                var point = transform.GetChild(index - 1);

                hero.gameObject = Instantiate(hero.puppet.prefab, Vector3.zero, Quaternion.identity) as GameObject;
                hero.transform.localScale = Vector3.one;
                hero.transform.SetParent(point, false);

                _heroes.Add(hero);

                return true;
            }

            return false;
        }        

        private void DeathHeroHandler(Hero sender)
        {
            sender.OnDeath -= DeathHeroHandler;
            _heroes.Remove(sender);
        }

        private IEnumerator Tweaking(GroupCallback callback)
        {
            var count = _heroes.Count;
            var list = new List<Transform>(count);

            for (var i = 0; i < count; ++i)
            {
                var hero = _heroes[i];
                var point = transform.GetChild(i);

                var diff = hero.transform.position.x - point.position.x;
                if (diff > Mathf.Epsilon)
                {
                    hero.transform.SetParent(point, true);
                    list.Add(hero.transform);
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

            if (callback != null)
            {
                callback(this);
            }
        }

    }

}
