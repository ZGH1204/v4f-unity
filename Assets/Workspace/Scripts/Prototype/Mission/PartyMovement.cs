// <copyright file="PartyMovement.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

namespace V4F.Prototype.Mission
{

    public class PartyMovement : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private Transform _transform;
        private Vector3 _position;
        private IEnumerator _move;

        private void MovementHandler(Vector3 data, bool immediately)
        {
            _position.x = data.x;

            if (immediately)
            {
                _transform.localPosition = _position;
            }
            else
            {
                PlayMovement();
            }
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
            var wait = new WaitForFixedUpdate();

            var position = transform.position;

            while (Mathf.Abs(_rigidbody.position.x - _position.x) > Mathf.Epsilon)
            {                
                position.x = _position.x;
                _rigidbody.MovePosition(position);
                yield return wait;
            }
            
            position.x = _position.x;
            _rigidbody.position = position;

            _move = null;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _transform = GetComponent<Transform>();
        }

        private void OnEnable()
        {
            PartyController.OnMovement += MovementHandler;
        }

        private void OnDisable()
        {
            PartyController.OnMovement -= MovementHandler;
        }

        private void Start()
        {
            _position = _transform.localPosition;
        }
    }
	
}
