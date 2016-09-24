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
        private IEnumerator _rotateTweaking;
        private Vector3 _startPosition;
        private Vector3 _atPoint;
        private Vector3 _tweakPosition;

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

        private void BattleStartHandler(Battle sender)
        {
            _startPosition = _transform.localPosition;
            _atPoint = _transform.position + _transform.forward * -_transform.localPosition.z;
        }

        private void FocusGroupHandler(Battle sender)
        {
            if (sender.actor.controlAI)
            {
                _tweakPosition = _startPosition + new Vector3(0.8f, 0f, 0.02f);
            }
            else
            {
                _tweakPosition = _startPosition + new Vector3(-0.8f, 0f, 0.02f);
            }

            StopTweaking();

            if (_rotateTweaking == null)
            {
                _rotateTweaking = RotateTweaking();
                StartCoroutine(_rotateTweaking);
            }
        }

        private void UnfocusGroupHandler(Battle sender)
        {
            _tweakPosition = _startPosition;
            if (_rotateTweaking == null)
            {
                _rotateTweaking = RotateTweaking();
                StartCoroutine(_rotateTweaking);
            }
        }

        private void BattleEndHandler(Battle sender)
        {
            _tweakPosition = _startPosition;
            if (_rotateTweaking == null)
            {
                _rotateTweaking = RotateTweaking();
                StartCoroutine(_rotateTweaking);
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

        private IEnumerator RotateTweaking()
        {
            while (Vector3.Distance(_transform.localPosition, _tweakPosition) > 0.01f)
            {
                _transform.localPosition = Vector3.Lerp(_transform.localPosition, _tweakPosition, Time.deltaTime * 2f);
                _transform.LookAt(_atPoint);

                yield return null;
            }

            _transform.localPosition = _tweakPosition;
            _transform.LookAt(_atPoint);
            
            _rotateTweaking = null;
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _targetPosition = _transform.position;
        }

        private void OnEnable()
        {
            PartyController.OnMovement += MovementHandler;
            Battle.OnStart += BattleStartHandler;
            Battle.OnFocusGroup += FocusGroupHandler;
            Battle.OnUnfocusGroup += UnfocusGroupHandler;
            Battle.OnEnd += BattleEndHandler;
        }

        private void OnDisable()
        {
            PartyController.OnMovement -= MovementHandler;
            Battle.OnStart -= BattleStartHandler;
            Battle.OnFocusGroup -= FocusGroupHandler;
            Battle.OnUnfocusGroup -= UnfocusGroupHandler;
            Battle.OnEnd -= BattleEndHandler;
        }

        private void Start()
        {
            var camera = GetComponent<Camera>();            
            camera.fieldOfView = desiredHorizontalFOV * (16f / 9f) / ((float)camera.pixelWidth / camera.pixelHeight);
        }
    }
	
}
