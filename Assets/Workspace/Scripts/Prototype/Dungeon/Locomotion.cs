// <copyright file="Locomotion.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

namespace V4F.Prototype.Dungeon
{

    public class Locomotion : MonoBehaviour
    {
        public delegate void MoveEventHandler(Locomotion sender, Vector3 position);

        public static event MoveEventHandler OnMove;        

        [Range(0f, 100f)]
        public float speedForward = 3f;

        [Range(0f, 100f)]
        public float speedBackward = 1f;

        [Range(-10f, 10f)]
        public float offsetFollow = 4f;

        [Range(0f, 100f)]
        public float speedFollow = 3f;

        private Vector3[] _bounds;
        private Rigidbody _rigidbody;
        private Transform _transform;
        private IEnumerator _movement = null;
        private Vector3 _direction;
        private float _speed;
        private bool _move;
        
        public void Initialize(Vector3 positionBounds, Vector3 cameraBounds)
        {
            _bounds = new Vector3[] { positionBounds, cameraBounds };

            var startPosition = _transform.position;
            startPosition.x = positionBounds.z;
            _transform.position = startPosition;

            var cameraPosition = Mathf.Clamp(startPosition.x + offsetFollow, cameraBounds.x, cameraBounds.y);
            OnMoveCallback(this, new Vector3(cameraPosition, speedFollow, -1f));
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
            _speed = 0f;
            _move = false;            
        }

        private static void OnMoveCallback(Locomotion sender, Vector3 position)
        {
            if (OnMove != null)
            {
                OnMove(sender, position);
            }
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _transform = GetComponent<Transform>();
        }        

        private void OnEnable()
        {
            //TouchAdapter.OnTouchPress += OnTouchPress;
            //TouchAdapter.OnTouchUp += OnTouchUp;
        }

        private void OnDisable()
        {
            //TouchAdapter.OnTouchPress -= OnTouchPress;
            //TouchAdapter.OnTouchUp -= OnTouchUp;
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
            var wait = new WaitForFixedUpdate();

            while (_move)
            {
                var playerPosition = _transform.position + _direction * (_speed * Time.fixedDeltaTime);
                var playerBounds = _bounds[0];
                playerPosition.x = Mathf.Clamp(playerPosition.x, playerBounds.x, playerBounds.y);
                _rigidbody.MovePosition(playerPosition);

                var cameraBounds = _bounds[1];
                var cameraPosition = Mathf.Clamp(playerPosition.x + offsetFollow, cameraBounds.x, cameraBounds.y);
                OnMoveCallback(this, new Vector3(cameraPosition, speedFollow, 0f));

                yield return wait;
            }

            _movement = null;
        }
    }
	
}
