// <copyright file="Statistics.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using V4F.Character;

namespace V4F.UI.Valhalla
{

    public class Statistics : MonoBehaviour
    {
        public delegate void HeroFocusedEventHandler(Actor hero);
        public delegate void SkillFocusedEventHandler(Skill skill);

        public RectTransform content;
        public Stock stockList;
        public Barracks barracksList;
                
        public Button payButton;

        public RectTransform skills;
        public SkillCell[] skillCells = new SkillCell[4];        

        public event HeroFocusedEventHandler OnHeroFocused;
        public event SkillFocusedEventHandler OnSkillFocused;

        private RectTransform _self;
        private Vector2 _capturePosition = Vector2.zero;
        private Vector2 _halfPosition = Vector2.zero;
        private Vector2 _fullPosition = Vector2.zero;
        private Vector2 _beforPosition = Vector2.zero;
        private Vector2 _afterPosition = Vector2.zero;
        private Vector2 _tweakPosition = Vector2.zero;
        private float _step = 0f;
        private bool _capture = false;

        private IEnumerator _tweaking = null;

        private ActorCell _heroFocused;
        private Actor _hero;
        private int _heroIndex;
        private SkillCell _skillFocused;

        private bool capture
        {
            get { return _capture; }
            set
            {
                if (!_capture && value)
                {
                    StopTweaking();
                }
                else if (_capture && !value)
                {
                    var velocity = _afterPosition - _beforPosition;                    
                    if (Mathf.Abs(velocity.x) > 10f)
                    {
                        _tweakPosition = ((velocity.x > 0f) ? _halfPosition : _fullPosition);
                    }
                    else
                    {
                        var toFull = Mathf.Abs(content.anchoredPosition.x - _fullPosition.x);
                        var toHalf = Mathf.Abs(content.anchoredPosition.x - _halfPosition.x);
                        _tweakPosition = ((toFull < toHalf) ? _fullPosition : _halfPosition);
                    }

                    PlayTweaking(false);
                }

                _capture = value;
            }
        }        

        private void TouchDownHandler(TouchAdapter sender, TouchEventArgs args)
        {
            capture = RectTransformUtility.RectangleContainsScreenPoint(content, args.position, sender.camera);
            if (capture)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_self, args.position, sender.camera, out _beforPosition);
                _afterPosition = _beforPosition;
                _capturePosition = content.anchoredPosition;                
            }        
        }

        private void TouchUpHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (capture)
            {
                _beforPosition = _afterPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_self, args.position, sender.camera, out _afterPosition);                
            }

            capture = false;
        }

        private void TouchMoveHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (capture)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_self, args.position, sender.camera, out _afterPosition);

                var offset = _capturePosition.x + (_afterPosition.x - _beforPosition.x);
                _tweakPosition.x = Mathf.Clamp(offset, _fullPosition.x, _halfPosition.x);

                PlayTweaking(true);
            }
        }

        private void TouchTapHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(skills, args.position, sender.camera))
            {
                SkillCell focus = null;
                for (var i = 0; i < skillCells.Length; ++i)
                {
                    var verifiable = skillCells[i];
                    if (RectTransformUtility.RectangleContainsScreenPoint(verifiable.rect, args.position, sender.camera))
                    {
                        focus = verifiable;
                        break;
                    }
                }                                

                if ((focus != null) && (focus != _skillFocused))
                {
                    if (_skillFocused != null)
                    {
                        _skillFocused.focus = false;
                    }

                    _skillFocused = focus;
                    _skillFocused.focus = true;

                    OnSkillFocusedCallback(focus.subject);
                }
                else
                {
                    OnSkillFocusedCallback(null);
                }
            }
        }

        private void OnHeroFocusedCallback(Actor hero)
        {
            if (OnHeroFocused != null)
            {
                OnHeroFocused(hero);
            }
        }

        private void OnSkillFocusedCallback(Skill skill)
        {
            if (OnSkillFocused != null)
            {
                OnSkillFocused(skill);
            }
        }

        private void PlayTweaking(bool inMoving)
        {
            if (!inMoving)
            {
                _step = 0f;
            }

            if (_tweaking == null)
            {
                _tweaking = Tweaking();
                StartCoroutine(_tweaking);
            }
        }

        private void StopTweaking()
        {
            _step = 0f;

            if (_tweaking != null)
            {
                StopCoroutine(_tweaking);
                _tweaking = null;
            }
        }

        private IEnumerator Tweaking()
        {            
            while (_step < 1f)
            {
                _step = Mathf.Clamp01(_step + Time.deltaTime);
                content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, _tweakPosition, _step);
                yield return null;
            }

            content.anchoredPosition = _tweakPosition;

            _tweaking = null;
        }

        private void OnStockSelectHandler(ListBox<ActorCell> sender, ListBoxEventArgs args)
        {
            content.gameObject.SetActive(true);
            _heroIndex = args.index;            
            UpdateHeroStats(sender[_heroIndex], true);            
        }

        private void OnBarracksSelectHandler(ListBox<ActorCell> sender, ListBoxEventArgs args)
        {
            content.gameObject.SetActive(true);
            _heroIndex = -1;
            UpdateHeroStats(sender[args.index], false);            
        }

        private void UpdateHeroStats(ActorCell cell, bool payButton)
        {
            if (_heroFocused != null)
            {
                _heroFocused.focus = false;
                _heroFocused = null;
            }

            if (_skillFocused != null)
            {
                _skillFocused.focus = false;
                _skillFocused = null;
            }
            
            this.payButton.disable = !payButton;

            _hero = cell.subject;
            _heroFocused = cell;
                        
            var skillSet = _hero.puppet.skillSet;            
            for (var i = 0; i < skillCells.Length; ++i)
            {
                var skillCell = skillCells[i];
                skillCell.subject = ((i < skillSet.countSkills) ? skillSet[i] : null);
            }

            OnHeroFocusedCallback(_hero);
        }

        private void OnClickPayButton(Button sender, ButtonEventArgs args)
        {
            if (_heroIndex != -1)
            {
                _heroFocused.focus = false;                
                stockList.RemoveAt(_heroIndex);
                _heroFocused = null;
                _heroIndex = -1;

                content.gameObject.SetActive(true);

                var item = barracksList.Add();
                item.subject = _hero;
                item.focus = true;
                UpdateHeroStats(item, false);                
            }
        }

        private void Awake()
        {
            _self = GetComponent<RectTransform>();                        
            _halfPosition = content.anchoredPosition;
            _heroFocused = null;
        }

        private void Start()
        {
            content.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            TouchAdapter.OnTouchDown += TouchDownHandler;
            TouchAdapter.OnTouchMove += TouchMoveHandler;
            TouchAdapter.OnTouchUp += TouchUpHandler;
            TouchAdapter.OnTouchTap += TouchTapHandler;

            stockList.OnSelect += OnStockSelectHandler;
            barracksList.OnSelect += OnBarracksSelectHandler;
            payButton.OnClick += OnClickPayButton;
        }

        private void OnDisable()
        {            
            TouchAdapter.OnTouchDown -= TouchDownHandler;
            TouchAdapter.OnTouchMove -= TouchMoveHandler;
            TouchAdapter.OnTouchUp -= TouchUpHandler;
            TouchAdapter.OnTouchTap -= TouchTapHandler;

            stockList.OnSelect -= OnStockSelectHandler;
            barracksList.OnSelect -= OnBarracksSelectHandler;
            payButton.OnClick -= OnClickPayButton;
        }
    }
	
}