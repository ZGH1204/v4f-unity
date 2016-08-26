// <copyright file="Follow.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

namespace V4F.Prototype.Dungeon
{

    public class Follow : MonoBehaviour
    {
        public Locomotion motion;

        [Range(0f, 100f)]
        public float speedFollow = 15f;

        private IEnumerator _tweaking;
        private Vector3 _targetPosition;        

        private void OnMove(Locomotion sender, Vector3 position)
        {
            _targetPosition.x = Mathf.Clamp(position.x + 4.5f, -20f, 20f);
            PlayTweaking();
        }

        private void Start()
        {
            _targetPosition = transform.position;            
        }
	
        private void OnEnable()
        {
            motion.OnMove += OnMove;
        }

        private void OnDisable()
        {
            motion.OnMove -= OnMove;
        }

        private void PlayTweaking()
        {
            if (_tweaking == null)
            {
                _tweaking = Tweaking();
                StartCoroutine(_tweaking);
            }
        }

        private IEnumerator Tweaking()
        {
            while (Vector3.Distance(transform.position, _targetPosition) > Mathf.Epsilon)
            {
                transform.position = Vector3.Lerp(transform.position, _targetPosition, speedFollow * Time.deltaTime);
                yield return null;
            }

            transform.position = _targetPosition;

            _tweaking = null;
        }
    }
	
}
