// <copyright file="Follow.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

namespace V4F.Prototype.Mission
{

    public class Follow : MonoBehaviour
    {
        public float desiredHorizontalFOV = 50f;

        private Transform _transform;
        private IEnumerator _tweaking;
        private Vector3 _targetPosition;
        private float _speedFollow;

        private void MovementHandler(Vector3 data, bool immediately)
        {           
            _targetPosition.x = data.y;
            if (immediately)
            {
                StopTweaking();
                _speedFollow = 0f;
                _transform.position = _targetPosition;
            }
            else
            {
                _speedFollow = data.z;
                PlayTweaking();
            }            
        }        

        private void PlayTweaking()
        {
            if (_tweaking == null)
            {
                _tweaking = Tweaking();
                StartCoroutine(_tweaking);
            }
        }

        private void StopTweaking()
        {
            if (_tweaking != null)
            {
                StopCoroutine(_tweaking);
                _tweaking = null;
            }
        }

        private IEnumerator Tweaking()
        {
            var wait = new WaitForFixedUpdate();

            while (Vector3.Distance(_transform.position, _targetPosition) > Mathf.Epsilon)
            {
                _transform.position = Vector3.Lerp(_transform.position, _targetPosition, _speedFollow * Time.fixedDeltaTime);
                yield return wait;
            }

            _transform.position = _targetPosition;
            _tweaking = null;
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _targetPosition = _transform.position;
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
            var camera = GetComponent<Camera>();
            camera.fieldOfView = desiredHorizontalFOV * (16f / 9f) / ((float)camera.pixelWidth / camera.pixelHeight);
        }
    }
	
}
