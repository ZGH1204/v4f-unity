// <copyright file="Statistics.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using V4F.Game;
using V4F.Puppets;

namespace V4F.UI.Valhalla
{

    public class Statistics : MonoBehaviour
    {
        public RectTransform content;
        public Stock stockList;
        public Barracks barracksList;

        public Text specInfo;
        public Button payButton;

        public RectTransform skills;
        public SkillCell skill1;
        public SkillCell skill2;
        public SkillCell skill3;
        public SkillCell skill4;

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

        private HeroCell _heroFocused;
        private Hero _hero;
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
                SkillCell skill = null;

                if (RectTransformUtility.RectangleContainsScreenPoint(skill1.rect, args.position, sender.camera))
                {
                    skill = skill1;
                }
                if (RectTransformUtility.RectangleContainsScreenPoint(skill2.rect, args.position, sender.camera))
                {
                    skill = skill2;
                }
                if (RectTransformUtility.RectangleContainsScreenPoint(skill3.rect, args.position, sender.camera))
                {
                    skill = skill3;
                }
                if (RectTransformUtility.RectangleContainsScreenPoint(skill4.rect, args.position, sender.camera))
                {
                    skill = skill4;
                }

                if ((skill != null) && (skill != _skillFocused))
                {
                    if (_skillFocused != null)
                    {
                        _skillFocused.focus = false;
                    }

                    _skillFocused = skill;
                    _skillFocused.focus = true;
                }                
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

        private void OnStockSelectHandler(ListBox<HeroCell> sender, ListBoxEventArgs args)
        {
            _heroIndex = args.index;            
            UpdateHeroStats(sender[_heroIndex], true);
            content.gameObject.SetActive(true);
        }

        private void OnBarracksSelectHandler(ListBox<HeroCell> sender, ListBoxEventArgs args)
        {
            _heroIndex = -1;
            UpdateHeroStats(sender[args.index], false);
            content.gameObject.SetActive(true);
        }

        private void UpdateHeroStats(HeroCell cell, bool payButton)
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

            _heroFocused = cell;
            _hero = _heroFocused.subject;

            var spec = _hero.puppet.spec;
            specInfo.text = string.Format("Health Points  {0}\nAccuracy       {1}\nInitiative     {2}\nStamina        {3}",
                spec.GetStat(Stats.HealthPoints), spec.GetStat(Stats.Accuracy), spec.GetStat(Stats.Initiative), spec.GetStat(Stats.Stamina));

            this.payButton.disable = !payButton;

            var skillSet = _hero.puppet.skillSet;
            var skillIndex = 0;
            skill1.skill = (skillIndex < skillSet.countSkills) ? skillSet[skillIndex++] : null;
            skill2.skill = (skillIndex < skillSet.countSkills) ? skillSet[skillIndex++] : null;
            skill3.skill = (skillIndex < skillSet.countSkills) ? skillSet[skillIndex++] : null;
            skill4.skill = (skillIndex < skillSet.countSkills) ? skillSet[skillIndex++] : null;            
        }

        private void OnClickPayButton(Button sender, ButtonEventArgs args)
        {
            if (_heroIndex != -1)
            {
                _heroFocused.focus = false;                
                stockList.RemoveAt(_heroIndex);
                _heroFocused = null;
                _heroIndex = -1;

                var item = barracksList.Add();
                item.subject = _hero;
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