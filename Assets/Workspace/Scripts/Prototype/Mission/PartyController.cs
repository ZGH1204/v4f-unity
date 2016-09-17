// <copyright file="PartyController.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

using V4F.FSM;
using V4F.Prototype.Map;

namespace V4F.Prototype.Mission
{

    public class PartyController : MonoBehaviour
    {
        public delegate void MovementEventHandler(Vector3 position, bool immediately);
        public static event MovementEventHandler OnMovement;

        public TouchAdapter input;

        public GameObject corridor;
        public GameObject room;
        public GameObject party;

        [Range(0f, 100f)]
        public float speedForward = 3f;

        [Range(0f, 100f)]
        public float speedBackward = 1f;

        [Range(-10f, 10f)]
        public float offset = 4f;

        [Range(0f, 100f)]
        public float follow = 3f;

        private BorderMovement _corridor;
        private BorderMovement _room;        
        private Transform _transform;

        private GameObject _location;
        private Vector2 _movement;
        private Vector2 _anchors;
        private Vector3 _direction;
        private float _speed;

        private IEnumerator _move;
        private bool _loopMovement;

        private static void OnMovementCallback(Vector3 position, bool immediately)
        {
            if (OnMovement != null)
            {
                OnMovement(position, immediately);
            }
        }        

        public void Entry(NodeType type)
        {
            BorderMovement border = null;

            if (type != NodeType.Room)
            {
                _location = corridor;
                border = _corridor;
            }
            else
            {
                _location = room;
                border = _room;
            }

            _location.SetActive(true);
            _movement = border.movement;
            _anchors = border.anchors;

            OnMovementCallback(new Vector3(_movement.x, _anchors.x, follow), true);

            enabled = true;
        }

        public void Resume()
        {            
            enabled = true;
        }

        public void Pause()
        {
            _loopMovement = false;

            enabled = false;
        }

        public void Exit()
        {
            _location.SetActive(false);
            _loopMovement = false;

            enabled = false;
        }

        private void OnTouchPress(TouchAdapter sender, TouchEventArgs args)
        {
            var half = Screen.width * 0.5f;
            if (args.position.x < half)
            {
                _direction = Vector3.left;
                _speed = speedBackward;
            }
            else
            {
                _direction = Vector3.right;
                _speed = speedForward;
            }
            
            PlayMovement();
        }

        private void OnTouchUp(TouchAdapter sender, TouchEventArgs args)
        {
            _loopMovement = false;
        }

        private void PlayMovement()
        {            
            if (_move == null)
            {
                _move = Movement();
                StartCoroutine(_move);
            }
        }

        private IEnumerator Movement()
        {
            _loopMovement = true;
            while (_loopMovement)
            {
                var point = _transform.localPosition + _direction * (_speed * Time.fixedDeltaTime);
                var position = Mathf.Clamp(point.x, _movement.x, _movement.y);
                var anchor = Mathf.Clamp(point.x + offset, _anchors.x, _anchors.y);

                OnMovementCallback(new Vector3(position, anchor, follow), false);

                yield return null;
            }

            _move = null;
        }

        private void Awake()
        {
            _corridor = corridor.GetComponent<BorderMovement>();
            _room = room.GetComponent<BorderMovement>();
            _transform = party.GetComponent<Transform>();
        }

        private void OnEnable()
        {
            input.OnTouchPress += OnTouchPress;
            input.OnTouchUp += OnTouchUp;            
        }

        private void OnDisable()
        {
            input.OnTouchPress -= OnTouchPress;
            input.OnTouchUp -= OnTouchUp;
        }

    }
	
}
