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
        private static readonly int __anim_param_speed = Animator.StringToHash("speed");
        private static readonly int __anim_param_direction = Animator.StringToHash("direction");

        private List<Actor> _heroes;

        private Transform _transform;
        private IEnumerator _movement;
        private float _speed;
        private float _direction;
        private float _position;
        private bool _move;

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
                b.orderIndex = i1;

                _heroes[i2] = a;                
                a.orderIndex = i2;

                return true;
            }

            return false;
        }

        public void Prepare(int count)
        {
            _heroes = new List<Actor>(count);
            _movement = null;
            _speed = 0f;
            _move = false;
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
                hero.orderIndex = index - 1;

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
                if (Mathf.Abs(diff) > Mathf.Epsilon)
                {
                    hero.transform.SetParent(point, true);
                    hero.orderIndex = i;
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

        private void MovementStartHandler(Vector3 data, bool immediately)
        {            
            _position = _transform.localPosition.x;
            _direction = 1f;

            PlayMovement();
        }

        private void MovementEndHandler(Vector3 data, bool immediately)
        {
            StopMovement();
        }

        private void MovementHandler(Vector3 data, bool immediately)
        {
            var diff = _transform.localPosition.x - _position;

            if (diff < 0f)
            {
                if (_direction > 0f)
                {
                    _direction = -1f;
                    var count = _heroes.Count;
                    for (var i = 0; i < count; ++i)
                    {
                        var animator = _heroes[i].animator;
                        animator.SetFloat(__anim_param_direction, _direction);
                    }
                }
            }
            else if (diff > 0f)
            {
                if (_direction < 0f)
                {
                    _direction = 1f;
                    var count = _heroes.Count;
                    for (var i = 0; i < count; ++i)
                    {
                        var animator = _heroes[i].animator;
                        animator.SetFloat(__anim_param_direction, _direction);
                    }
                }
            }            

            _position = _transform.localPosition.x;
        }

        private void PlayMovement()
        {            
            if (_movement != null)
            {
                StopCoroutine(_movement);
            }

            _movement = MovementAnimation();
            _move = true;

            StartCoroutine(_movement);
        }

        private void PlayMovementIfStopped()
        {
            if (_movement == null)
            {
                _movement = MovementAnimation();
                _move = true;

                StartCoroutine(_movement);                
            }
        }

        private void StopMovement()
        {
            _move = false;            
        }

        private IEnumerator MovementAnimation()
        {
            var count = _heroes.Count;

            while (_speed < 1f)
            {
                _speed = Mathf.Clamp01(_speed + Time.deltaTime * 10f);                
                for (var i = 0; i < count; ++i)
                {
                    var animator = _heroes[i].animator;
                    animator.SetFloat(__anim_param_speed, _speed);
                }

                yield return null;
            }

            while (_move)
            {
                yield return null;
            }

            while (_speed > 0f)
            {
                _speed = Mathf.Clamp01(_speed - Time.deltaTime * 10f);                
                for (var i = 0; i < count; ++i)
                {
                    var animator = _heroes[i].animator;
                    animator.SetFloat(__anim_param_speed, _speed);
                }

                yield return null;
            }                       

            _movement = null;
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        private void OnEnable()
        {
            PartyController.OnMovementStart += MovementStartHandler;
            PartyController.OnMovementEnd += MovementEndHandler;
            PartyController.OnMovement += MovementHandler;
        }

        private void OnDisable()
        {
            PartyController.OnMovementStart -= MovementStartHandler;
            PartyController.OnMovementEnd -= MovementEndHandler;
            PartyController.OnMovement -= MovementHandler;
        }

    }

}
