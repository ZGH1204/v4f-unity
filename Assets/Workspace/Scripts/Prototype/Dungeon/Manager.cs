// <copyright file="Manager.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

using V4F.Prototype.Map;

namespace V4F.Prototype.Dungeon
{

    public class Manager : MonoBehaviour
    {
        public delegate void ChangeScreenSizeHandler();
        public static event ChangeScreenSizeHandler OnChangeScreenSize;

        public Data map;

        private IEnumerator _detect = null;
        private bool _process = false;
        private int _w = 0;
        private int _h = 0;

        private static void OnChangeScreenSizeCallback()
        {
            if (OnChangeScreenSize != null)
            {
                OnChangeScreenSize();
            }
        }        

        private void OnEnable()
        {
            _detect = DetectChangeScreenSize();
            StartCoroutine(_detect);
        }

        private void OnDisable()
        {
            StopCoroutine(_detect);
            _detect = null;
        }

        private void Start()
        {
            _process = true;
        }
	
        private IEnumerator DetectChangeScreenSize()
        {
            while (!_process)
            {
                yield return null;
            }

            while (_process)
            {                
                if (Screen.width != _w || Screen.height != _h)
                {
                    _w = Screen.width;
                    _h = Screen.height;
                    OnChangeScreenSizeCallback();
                }
                yield return null;
            }
        }
    }
	
}
