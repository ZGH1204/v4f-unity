// <copyright file="Follow.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

namespace V4F.Prototype.Dungeon
{

    public class Follow : MonoBehaviour
    {
        public float desiredHorizontalFOV = 50f;

        private Transform _transform;
        private IEnumerator _tweaking;
        private Vector3 _targetPosition;
        private float _speedFollow;

        private void OnMove(Locomotion sender, Vector3 position)
        {           
            _targetPosition.x = position.x;            
            if (position.z < 0f)
            {
                StopTweaking();
                _speedFollow = 0f;
                _transform.position = _targetPosition;
            }
            else
            {
                _speedFollow = position.y;
                PlayTweaking();
            }            
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _targetPosition = _transform.position;
        }        
	
        private void OnEnable()
        {
            Locomotion.OnMove += OnMove;
            Manager.OnChangeScreenSize += OnChangeScreenSize;
        }

        private void OnDisable()
        {
            Locomotion.OnMove -= OnMove;
            Manager.OnChangeScreenSize -= OnChangeScreenSize;
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

        private void OnChangeScreenSize()
        {
            var camera = GetComponent<Camera>();
            camera.fieldOfView = desiredHorizontalFOV * (16f / 9f) / ((float)camera.pixelWidth / camera.pixelHeight);
        }
    }
	
}
