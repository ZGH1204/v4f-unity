// <copyright file="Locomotion.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

namespace V4F.Prototype.Dungeon
{

    public class Locomotion : MonoBehaviour
    {
        public delegate void MoveEventHandler(Locomotion sender, Vector3 position);

        public event MoveEventHandler OnMove;

        [Range(0f, 100f)]
        public float speedForward = 3f;

        [Range(0f, 100f)]
        public float speedBackward = 1f;

        private IEnumerator _movement = null;
        private Vector3 _targetPosition;        
        private float _speed;
        private bool _move;

        private void OnTouchPress(TouchAdapter sender, TouchEventArgs args)
        {            
            var half = Screen.width * 0.5f;
            if (args.position.x < half)
            {
                _targetPosition = transform.position;
                _targetPosition.x = -25f;
                _speed = speedBackward;
            }
            else
            {
                _targetPosition = transform.position;
                _targetPosition.x = 25f;
                _speed = speedForward;
            }

            PlayMovement();
        }

        private void OnTouchUp(TouchAdapter sender, TouchEventArgs args)
        {
            _move = false;
            _speed = 0f;
        }

        private void OnMoveCallback(Vector3 position)
        {
            if (OnMove != null)
            {
                OnMove(this, position);
            }
        }

        private void OnEnable()
        {
            TouchAdapter.OnTouchPress += OnTouchPress;
            TouchAdapter.OnTouchUp += OnTouchUp;
        }

        private void OnDisable()
        {
            TouchAdapter.OnTouchPress -= OnTouchPress;
            TouchAdapter.OnTouchUp -= OnTouchUp;
        }

        private void PlayMovement()
        {
            _move = true;

            if (_movement == null)
            {
                _movement = Movement();
                StartCoroutine(_movement);
            }
        }

        private IEnumerator Movement()
        {
            while (_move)
            {                
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
                OnMoveCallback(transform.position);
                yield return null;                
            }

            _movement = null;
        }
    }
	
}
