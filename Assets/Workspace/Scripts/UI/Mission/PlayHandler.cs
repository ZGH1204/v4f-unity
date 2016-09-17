// <copyright file="PlayHandler.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using V4F.Character;
using V4F.Prototype.Mission;
using V4F.Prototype.Map;

namespace V4F.UI.Mission
{

    public class PlayHandler : MonoBehaviour
    {
        public TouchAdapter input;
        public RectTransform canvas;
        public Camera cameraUI;
        public Camera cameraActors;

        public Battle battleState;

        public Section enter;
        public Section exit;
        public Section room;

        public Party party;

        public HealthBarHandler[] healthForHeroes = new HealthBarHandler[4];
        public HealthBarHandler[] healthForEnemies = new HealthBarHandler[4];

        public RectTransform[] frameForHeroes = new RectTransform[4];
        public RectTransform[] frameForEnemies = new RectTransform[4];

        public BattlePanelHandler battlePanel;
        public RectTransform selection;        

        public Button enterButton;
        public Button exitButton;
        public Button mapButton;

        private Actor _selected;
        private bool _entreButtonState;
        private bool _exitButtonState;
        private bool _mapButtonState;
        private bool _choose;

        private Button _lastButton;

        public void Entry(NodeType type)
        {
            gameObject.SetActive(true);
        }

        public void Exit()
        {
            enterButton.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(false);
            mapButton.gameObject.SetActive(false);
            gameObject.gameObject.SetActive(false);
        }

        public void EntryBattle()
        {
            _entreButtonState = enterButton.disable;
            enterButton.disable = true;
            _exitButtonState = exitButton.disable;
            exitButton.disable = true;
            _mapButtonState = mapButton.disable;
            mapButton.disable = true;

            selection.gameObject.SetActive(true);
            battleState.OnStage += OnBattleStage;
            battlePanel.locked = false;
            battlePanel.OnSelectSkill += SelectSkillHandler;

            input.OnTouchTap += TouchTapHandler;
        }

        public void ExitBattle()
        {
            input.OnTouchTap -= TouchTapHandler;

            _selected = null;

            battlePanel.OnSelectSkill -= SelectSkillHandler;
            battlePanel.locked = true;
            battleState.OnStage -= OnBattleStage;
            selection.gameObject.SetActive(false);

            enterButton.disable = _entreButtonState;
            exitButton.disable = _exitButtonState;
            mapButton.disable = _mapButtonState;
        }

        private void SectionEnterHandler(Section sender, SectionEventArgs args)
        {
            if (_lastButton != null)
            {                
                _lastButton.disable = true;
            }

            if (args.type == SectionType.Enter)
            {                
                if (_lastButton != null)
                {
                    _lastButton.gameObject.SetActive(false);
                }
                enterButton.gameObject.SetActive(true);
                enterButton.disable = false;
                _lastButton = enterButton;
            }
            else if (args.type == SectionType.Exit)
            {
                if (_lastButton != null)
                {
                    _lastButton.gameObject.SetActive(false);
                }                
                exitButton.gameObject.SetActive(true);
                exitButton.disable = false;
                _lastButton = exitButton;
            }
            else if (args.type == SectionType.Room)
            {
                if (_lastButton != null)
                {
                    _lastButton.gameObject.SetActive(false);
                }
                mapButton.gameObject.SetActive(true);
                mapButton.disable = false;
                _lastButton = mapButton;
            }
        }

        private void SectionExitHandler(Section sender, SectionEventArgs args)
        {            
            if (args.type == SectionType.Enter)
            {                
                enterButton.disable = true;
            }
            else if (args.type == SectionType.Exit)
            {
                exitButton.disable = true;
            }
            else if (args.type == SectionType.Room)
            {
                mapButton.disable = true;
            }
        }

        private void OnBattleStage(Battle sender)
        {
            _selected = sender.actor;
            battlePanel.SetActor(_selected);
            _choose = !_selected.controlAI;
        }

        private IEnumerator HealthBar()
        {
            var offset = new Vector2(0f, -15f);

            while (true)
            {
                var count = Mathf.Min(party.count, healthForHeroes.Length);
                for (var i = 0; i < count; ++i)
                {
                    var actor = party[i];
                    var screenPoint = cameraActors.WorldToScreenPoint(actor.transform.position);

                    Vector2 position;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, cameraUI, out position))
                    {
                        var hpb = healthForHeroes[i];
                        hpb.Enable();                        
                        hpb.SetValue(actor.healthPoint, actor[AttributeType.HealthPoints].value);
                        hpb.SetPosition(position + offset);
                        
                        frameForHeroes[i].anchoredPosition = position;
                    }
                }

                for (var i = count; i < healthForHeroes.Length; ++i)
                {
                    healthForHeroes[i].Disable();
                    frameForHeroes[i].gameObject.SetActive(false);
                }

                if (_selected != null)
                {
                    var screenPoint = cameraActors.WorldToScreenPoint(_selected.transform.position);
                    Vector2 position;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, cameraUI, out position))
                    {
                        selection.anchoredPosition = position;
                    }
                }                

                yield return null;
            }            
        }

        private IEnumerator HealthBar2()
        {
            var offset = new Vector2(0f, -15f);            

            while (true)
            {
                var enemies = battleState.enemies;
                var count = 0;

                if ((enemies != null) && (enemies.count > 0))
                {
                    count = Mathf.Min(enemies.count, healthForEnemies.Length);
                    for (var i = 0; i < count; ++i)
                    {
                        var actor = enemies[i];
                        var screenPoint = cameraActors.WorldToScreenPoint(actor.transform.position);

                        Vector2 position;
                        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, cameraUI, out position))
                        {
                            var hpb = healthForEnemies[i];
                            hpb.Enable();
                            hpb.SetValue(actor.healthPoint, actor[AttributeType.HealthPoints].value);
                            hpb.SetPosition(position + offset);

                            frameForEnemies[i].anchoredPosition = position;
                        }
                    }                    
                }

                for (var i = count; i < healthForEnemies.Length; ++i)
                {
                    healthForEnemies[i].Disable();
                    frameForEnemies[i].gameObject.SetActive(false);
                }

                yield return null;
            }            
        }

        private void SelectSkillHandler(int number)
        {
            if (number == -1)
            {
                for (var i = 0; i < 4; ++i)
                {
                    frameForHeroes[i].gameObject.SetActive(false);
                    frameForEnemies[i].gameObject.SetActive(false);
                }

                return;
            }

            var skill = _selected.puppet.skillSet[number][0];

            var goals = new List<bool>(4);
            for (var i = 0; i < 4; ++i)
            {
                goals.Add(IsPositionForTarget(skill.position, i + 1));
            }
            
            if (skill.goal == SkillGoal.Enemies)
            {
                for (var i = 0; i < 4; ++i)
                {
                    frameForHeroes[i].gameObject.SetActive(false);
                }

                for (var i = 0; i < 4; ++i)
                {
                    if (goals[i])
                    {
                        frameForEnemies[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        frameForEnemies[i].gameObject.SetActive(false);
                    }                    
                }
            }
            else
            {                
                for (var i = 0; i < 4; ++i)
                {
                    frameForEnemies[i].gameObject.SetActive(false);
                }

                for (var i = 0; i < 4; ++i)
                {
                    if (goals[i])
                    {
                        frameForHeroes[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        frameForHeroes[i].gameObject.SetActive(false);
                    }
                }
            }            
        }

        private bool IsPositionForTarget(int position, int positionNumber)
        {
            var number = positionNumber - 1;
            if (number == Mathf.Clamp(number, 0, 3))
            {
                return ((position & ((1 << number) << 4)) != 0);
            }

            return false;
        }

        private void TouchTapHandler(TouchAdapter sender, TouchEventArgs args)
        {
            if (_choose)
            {
                for (var i = 0; i < 4; ++i)
                {
                    var rect = frameForHeroes[i];
                    if (rect.gameObject.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(rect, args.position, cameraUI))
                    {
                        var goal = battleState.heroes[i];
                        var skill = _selected.puppet.skillSet[battlePanel.skillNumber][0];
                        battleState.AddAction(new Action(skill, goal));
                        _choose = false;
                        return;
                    }
                }

                for (var i = 0; i < 4; ++i)
                {
                    var rect = frameForEnemies[i];
                    if (rect.gameObject.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(rect, args.position, cameraUI))
                    {
                        var goal = battleState.enemies[i];
                        var skill = _selected.puppet.skillSet[battlePanel.skillNumber][0];
                        battleState.AddAction(new Action(skill, goal));
                        _choose = false;
                        return;
                    }
                }
            }
        }

        private void OnEnable()
        {            
            enter.OnEnter += SectionEnterHandler;
            enter.OnExit += SectionExitHandler;
            exit.OnEnter += SectionEnterHandler;
            exit.OnExit += SectionExitHandler;
            room.OnEnter += SectionEnterHandler;
            room.OnExit += SectionExitHandler;

            StartCoroutine(HealthBar());
            StartCoroutine(HealthBar2());
        }

        private void OnDisable()
        {
            StopAllCoroutines();

            enter.OnEnter -= SectionEnterHandler;
            enter.OnExit -= SectionExitHandler;
            exit.OnEnter -= SectionEnterHandler;
            exit.OnExit -= SectionExitHandler;
            room.OnEnter -= SectionEnterHandler;
            room.OnExit -= SectionExitHandler;
        }        
    }
	
}
