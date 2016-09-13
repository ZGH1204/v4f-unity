// <copyright file="TouchAdapter.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

namespace V4F
{

    public class TouchAdapter : MonoBehaviour
    {
        public delegate void TouchEventHandler(TouchAdapter sender, TouchEventArgs args);
        public delegate void BackEventHandler(TouchAdapter sender);

        #region Constants
        public const int MaxNumFingers = 10;
        #endregion

        #region Events
        public event TouchEventHandler OnTouchDown;
        public event TouchEventHandler OnTouchUp;
        public event TouchEventHandler OnTouchPress;
        public event TouchEventHandler OnTouchMove;
        public event TouchEventHandler OnTouchLong;
        public event TouchEventHandler OnTouchTap;

        public event BackEventHandler OnBack;
        #endregion

        #region Fields
        private static Stack<TouchAdapter> __stack = null;
        private static TouchAdapter __active = null;

        [SerializeField, HideInInspector]
        private int _countButtons = 1;

        [SerializeField, HideInInspector]
        private int _countFingers = 1;

        [SerializeField, HideInInspector]
        private float _allowableDisplacement = 0.5f;

        [SerializeField, HideInInspector]
        private float _longTouchTimer = 1f;

        [SerializeField, HideInInspector]
        public bool _longTouch = false;

        private Vector3[] _coords = new Vector3[MaxNumFingers];        
        private float[] _timers = new float[MaxNumFingers];
        private int[] _states = new int[MaxNumFingers];
        private Camera _camera = null;
        private bool _top = false;
        #endregion

        #region Properties
        public static TouchAdapter current
        {
            get { return __active; }
        }

        public new Camera camera
        {
            get { return __active._camera; }
        }

        private bool multiTouch
        {
            get { return(_countFingers > 1); }
        }

        private bool top
        {
            get { return _top; }
            set { _top = value; }
        }
        #endregion

        #region Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInitializeBefore()
        {
            __stack = new Stack<TouchAdapter>(32);
            __active = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RuntimeInitializeAfter()
        {
            
        }

        private static void Push(TouchAdapter adapter)
        {
            if (__active != null)
            {
                __stack.Push(__active);
                __active.top = false;
                __active.enabled = false;
            }
            
            Input.multiTouchEnabled = adapter.multiTouch;
            Input.simulateMouseWithTouches = false;

            __active = adapter;
            __active.top = true;
        }

        private static void Pop(TouchAdapter adapter)
        {
            if (adapter.top)
            {
                __active.top = false;
                __active = null;

                if (__stack.Count > 0)
                {
                    var prev = __stack.Pop();
                    prev.enabled = true;
                }
            }
        }

        private void OnTouchDownCallback(TouchEventArgs args)
        {
            if (OnTouchDown != null)
            {
                OnTouchDown(this, args);
            }                
        }

        private void OnTouchUpCallback(TouchEventArgs args)
        {
            if (OnTouchUp != null)
            {
                OnTouchUp(this, args);
            }
        }

        private void OnTouchPressCallback(TouchEventArgs args)
        {
            if (OnTouchPress != null)
            {
                OnTouchPress(this, args);
            }
        }

        private void OnTouchMoveCallback(TouchEventArgs args)
        {
            if (OnTouchMove != null)
            {
                OnTouchMove(this, args);
            }
        }

        private void OnTouchLongCallback(TouchEventArgs args)
        {
            if (OnTouchLong != null)
            {
                OnTouchLong(this, args);
            }
        }

        private void OnTouchTapCallback(TouchEventArgs args)
        {
            if (OnTouchTap != null)
            {
                OnTouchTap(this, args);
            }
        }

        private void OnBackCallback()
        {
            if (OnBack != null)
            {
                OnBack(this);
            }
        }

        private void UpdateFingersState()
        {
            var displacement = _allowableDisplacement * _allowableDisplacement;

#if UNITY_STANDALONE || UNITY_EDITOR

            for (var finger = 0; ((finger < 2) && (finger < _countButtons)); ++finger)
            {
                var eventSystem = EventSystem.current;
                if ((eventSystem != null) && eventSystem.IsPointerOverGameObject(finger))
                {
                    continue;
                }
                                
                var mouseOffset = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                var args = new TouchEventArgs(finger, Input.mousePosition, mouseOffset);

                if (Input.GetMouseButtonDown(finger))
                {
                    OnTouchDownCallback(args);

                    _coords[finger] = args.position;                    
                    _timers[finger] = _longTouchTimer;
                    _states[finger] = 1;

                    continue;
                }
                
                if (Input.GetMouseButtonUp(finger))
                {
                    var fingerState = _states[finger];
                    if (fingerState > 0)
                    {                                                
                        if (((fingerState & 0x02) == 0) && ((fingerState & 0x04) == 0))
                        {
                            OnTouchTapCallback(args);
                        }

                        OnTouchUpCallback(args);                        
                    }

                    _states[finger] = 0;

                    continue;
                }
                
                if (_states[finger] > 0)
                {
                    var offset = (args.position - _coords[finger]).sqrMagnitude;
                    var fingerState = _states[finger];                    

                    if ((fingerState & 0x02) == 0)
                    {                        
                        if (offset > displacement)
                        {
                            fingerState |= 0x02;
                        }
                        else if (_longTouch && ((fingerState & 0x04) == 0))
                        {
                            _timers[finger] -= Time.unscaledDeltaTime;
                            if (_timers[finger] < 0f)
                            {
                                OnTouchLongCallback(args);
                                fingerState |= 0x04;
                            }
                        }
                    }

                    if (((fingerState & 0x02) != 0) && (offset > 0f))
                    {
                        OnTouchMoveCallback(args);
                    }                                        

                    OnTouchPressCallback(args);

                    _states[finger] = fingerState;

                    continue;
                }
            }

#elif UNITY_ANDROID

            var touches = Input.touches;
            for (var i = 0; ((i < Input.touchCount) && (i < _countFingers)); ++i)
            {
                var touch = touches[i];

                var eventSystem = EventSystem.current;
                if ((eventSystem != null) && eventSystem.IsPointerOverGameObject(touch.fingerId))
                {
                    continue;
                }

                var mousePosition = new Vector3(touch.position.x, touch.position.y);
                var mouseOffset = new Vector3(touch.deltaPosition.x, touch.deltaPosition.y);                
                var args = new TouchEventArgs(touch.fingerId, mousePosition, mouseOffset);
                
                if (touch.phase == TouchPhase.Began)
                {
                    OnTouchDownCallback(args);

                    _coords[touch.fingerId] = args.position;                    
                    _timers[touch.fingerId] = _longTouchTimer;
                    _states[touch.fingerId] = 1;

                    continue;
                }
                
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    var fingerState = _states[touch.fingerId];
                    if (fingerState > 0)
                    {
                        if (((fingerState & 0x02) == 0) && ((fingerState & 0x04) == 0))
                        {
                            OnTouchTapCallback(args);
                        }

                        OnTouchUpCallback(args);
                    }

                    _states[touch.fingerId] = 0;

                    continue;
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    if (_states[touch.fingerId] > 0)
                    {
                        var offset = (args.position - _coords[touch.fingerId]).sqrMagnitude;
                        var fingerState = _states[touch.fingerId];

                        if ((fingerState & 0x02) == 0)
                        {
                            if (offset > displacement)
                            {
                                fingerState |= 0x02;
                            }
                            else if (_longTouch && ((fingerState & 0x04) == 0))
                            {
                                _timers[touch.fingerId] -= Time.unscaledDeltaTime;
                                if (_timers[touch.fingerId] < 0f)
                                {
                                    OnTouchLongCallback(args);
                                    fingerState |= 0x04;
                                }
                            }
                        }

                        if (((fingerState & 0x02) != 0) && (offset > 0f))
                        {
                            OnTouchMoveCallback(args);
                        }

                        OnTouchPressCallback(args);

                        _states[touch.fingerId] = fingerState;
                    }                    

                    continue;
                }
                
                if (touch.phase == TouchPhase.Stationary)
                {                    
                    if (_states[touch.fingerId] > 0)
                    {
                        var fingerState = _states[touch.fingerId];

                        if (_longTouch && ((fingerState & 0x02) == 0) && ((fingerState & 0x04) == 0))
                        {
                            _timers[touch.fingerId] -= Time.unscaledDeltaTime;
                            if (_timers[touch.fingerId] < 0f)
                            {
                                OnTouchLongCallback(args);
                                fingerState |= 0x04;
                            }
                        }

                        OnTouchPressCallback(args);

                        _states[touch.fingerId] = fingerState;
                    }

                    continue;
                }
            }

#endif
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            Push(this);
        }

        private void OnDisable()
        {
            Pop(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackCallback();
            }

            UpdateFingersState();
        }
        #endregion
    }

}
