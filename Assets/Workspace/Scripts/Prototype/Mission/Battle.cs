// <copyright file="Battle.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using V4F.FSM;
using V4F.UI.Mission;
using V4F.Character;

namespace V4F.Prototype.Mission
{

    public class Battle : State
    {
        public delegate void BattleEventHandler(Battle sender);

        public event BattleEventHandler OnWin;
        public event BattleEventHandler OnLose;
        public event BattleEventHandler OnRound;
        public event BattleEventHandler OnStage;

        public Party heroes;
        public Camera zoom;

        public StateTransition transition;
        public State state;
        
        private Squad _enemies;

        private List<Actor> _queueHeroes;
        private List<Actor> _queueEnemies;
        private Group _group;
        private Actor _actor;
        private int _roundNumber;

        private IEnumerator _watchProcces;
        private bool _watch;

        private List<Action> _actionStack;
        private Dictionary<Actor, int> _onDamage;

        public Squad enemies
        {
            get { return _enemies; }
            set { _enemies = value; }
        }

        public int roundNumber
        {
            get { return _roundNumber; }
        }

        public Group group
        {
            get { return _group; }
        }

        public Actor actor
        {
            get { return _actor; }
        }

        public override void EntryStart()
        {
            _queueHeroes = new List<Actor>(heroes.count);
            _queueEnemies = new List<Actor>(enemies.count);
            _actionStack = new List<Action>(8);
            _onDamage = new Dictionary<Actor, int>(8);
        }

        public override void EntryEnd()
        {
            _roundNumber = -1;

            OnStage += StageHandler;
        }

        public override void Exit()
        {
            OnStage -= StageHandler;
        }

        public override IEnumerable Execute()
        {            
            while (heroes.isAlive && enemies.isAlive)
            {
                FillQueues();

                OnRoundCallback();

                while ((_queueHeroes.Count > 0) || (_queueEnemies.Count > 0))
                {
                    Actor hero = null;
                    int heroInitiative;
                    var heroIndex = GetHeroFromQueue(out heroInitiative, out hero);
                    
                    Actor enemy = null;
                    int enemyInitiative;
                    var enemyIndex = GetEnemyFromQueue(out enemyInitiative, out enemy);
                    
                    if (enemyInitiative < heroInitiative)
                    {
                        _group = heroes;
                        _actor = hero;
                        _queueHeroes.RemoveAt(heroIndex);                        
                    }
                    else if (heroInitiative < enemyInitiative)
                    {
                        _group = enemies;
                        _actor = enemy;
                        _queueEnemies.RemoveAt(enemyIndex);
                    }
                    else
                    {
                        var happening = Random.Range(0, 100);
                        if (happening < 50)
                        {
                            _group = enemies;
                            _actor = enemy;
                            _queueEnemies.RemoveAt(enemyIndex);
                        }
                        else
                        {
                            _group = heroes;
                            _actor = hero;
                            _queueHeroes.RemoveAt(heroIndex);                            
                        }
                    }

                    if (_actor.healthPoint > 0)
                    {
                        OnStageCallback();

                        while (!(_actionStack.Count > 0))
                        {
                            yield return null;
                        }

                        while (_actionStack.Count > 0)
                        {
                            var action = _actionStack[0];
                            _actionStack.RemoveAt(0);

                            var damage = 0;
                            if (ApplySkill(action.goal, _actor, action.skill, out damage))
                            {
                                _onDamage.Add(action.goal, damage);
                            }
                        }

                        if (_onDamage.Count > 0)
                        {
                            _watchProcces = Watch().GetEnumerator();
                            _watch = true;
                        }

                        while (_watch)
                        {
                            if (_watchProcces != null)
                            {
                                yield return _watchProcces.Current;
                                _watch = _watchProcces.MoveNext();
                            }
                            else
                            {
                                yield return null;
                            }
                        }

                        foreach (var pair in _onDamage)
                        {
                            var actor = pair.Key;
                            if (actor.TakeDamage(pair.Value))
                            {
                                actor.Destroy();
                            }
                        }

                        _watch = true;
                        heroes.PlayTweaking(GroupCallback);                        
                        while (_watch)
                        {
                            yield return null;
                        }

                        _watch = true;
                        enemies.PlayTweaking(GroupCallback);
                        while (_watch)
                        {
                            yield return null;
                        }

                        if (!(heroes.isAlive && enemies.isAlive))
                        {
                            break;
                        }
                    }                    
                }                
            }

            enemies = null;

            if (heroes.isAlive)
            {
                OnWinCallback();

                var args = new StateEventArgs(state, transition);
                OnTransitionCallback(args);
            }
            else
            {
                OnLoseCallback();
            }
        }

        public void AddAction(Action action)
        {
            _actionStack.Add(action);
        }

        private void FillQueues()
        {
            for (var i = 0; i < heroes.count; ++i)
            {
                _queueHeroes.Add(heroes[i]);
            }

            for (var i = 0; i < enemies.count; ++i)
            {
                _queueEnemies.Add(enemies[i]);
            }
        }

        private int GetHeroFromQueue(out int initiative, out Actor actor)
        {                        
            var index = -1;

            initiative = -1;
            actor = null;            

            for (var i = 0; i < _queueHeroes.Count; ++i)
            {
                var a = _queueHeroes[i];
                if (a.initiative > initiative)
                {
                    initiative = a.initiative;
                    actor = a;
                    index = i;
                }
            }            

            return index;
        }

        private int GetEnemyFromQueue(out int initiative, out Actor actor)
        {
            var index = -1;

            initiative = -1;
            actor = null;

            for (var i = 0; i < _queueEnemies.Count; ++i)
            {
                var a = _queueEnemies[i];
                if (a.initiative > initiative)
                {
                    initiative = a.initiative;
                    actor = a;
                    index = i;
                }
            }

            return index;
        }    
        
        private bool ApplySkill(Actor target, Actor sender, SkillStage skill, out int damage)
        {            
            var calculate = true;
            var minDamage = 0;
            var maxDamage = 0;            

            switch (skill.damageType)
            {
                case DamageType.Melee:
                    minDamage = sender[AttributeType.MinDamageMelee].value;
                    maxDamage = sender[AttributeType.MaxDamageMelee].value;
                    break;

                case DamageType.Range:
                    minDamage = sender[AttributeType.MinDamageRange].value;
                    maxDamage = sender[AttributeType.MaxDamageRange].value;
                    break;

                case DamageType.Magic:
                    minDamage = sender[AttributeType.MinDamageMagic].value;
                    maxDamage = sender[AttributeType.MaxDamageMagic].value;
                    break;

                default:
                    calculate = false;
                    break;
            }

            damage = -1;

            if (calculate)
            {
                var chanceToDodge = target[AttributeType.ChanceToDodge].value;
                if (Random.Range(0, 100) > chanceToDodge)
                {
                    damage = Random.Range(minDamage, maxDamage + 1);
                    damage += Mathf.RoundToInt(damage * (skill.damageModifier / 100f));

                    var chanceToCrit = sender[AttributeType.ChanceToCrit].value;
                    if (Random.Range(0, 100) < chanceToCrit)
                    {
                        damage *= 2;
                    }
                }                
            }
            else if (_group.Change(target, sender))
            {
                _group.PlayTweaking(GroupCallback);
                _watch = true;
            }            

            return calculate;
        }

        private IEnumerable Watch()
        {
            var layer = LayerMask.NameToLayer("Zoom");
            foreach (var pair in _onDamage)
            {
                var actor = pair.Key;
                actor.gameObject.layer = layer;
            }

            _actor.gameObject.layer = layer;

            zoom.gameObject.SetActive(true);

            var offsetLeft = new Vector3(-1f, 0f, 0f);
            var offsetLeft2 = new Vector3(-1.8f, 0f, 0f);
            var offsetRight = new Vector3(1f, 0f, 0f);
            var offsetRight2 = new Vector3(1.8f, 0f, 0f);

            while (zoom.fieldOfView > 20f)
            {                
                zoom.fieldOfView -= Time.deltaTime * 75f;
                var step = Mathf.Clamp01((50f - zoom.fieldOfView) * 0.0333f);

                foreach (var pair in _onDamage)
                {
                    var actor = pair.Key;
                    if (actor.controlAI)
                    {
                        actor.transform.localPosition = (offsetLeft + offsetLeft2 * actor.transform.parent.GetSiblingIndex()) * step;
                    }
                    else
                    {
                        actor.transform.localPosition = (offsetRight + offsetRight2  * actor.transform.parent.GetSiblingIndex()) * step;
                    }
                    
                }

                if (_actor.controlAI)
                {
                    _actor.transform.localPosition = (offsetLeft + offsetLeft2 * actor.transform.parent.GetSiblingIndex()) * step;
                }
                else
                {
                    _actor.transform.localPosition = (offsetRight + offsetRight2 * actor.transform.parent.GetSiblingIndex()) * step;
                }

                yield return null;
            }

            zoom.fieldOfView = 20f;

            while (zoom.fieldOfView < 50f)
            {
                zoom.fieldOfView += Time.deltaTime * 75f;
                var step = Mathf.Clamp01((50f - zoom.fieldOfView) * 0.0333f);

                foreach (var pair in _onDamage)
                {
                    var actor = pair.Key;
                    if (actor.controlAI)
                    {
                        actor.transform.localPosition = (offsetLeft + offsetLeft2 * actor.transform.parent.GetSiblingIndex()) * step;
                    }
                    else
                    {
                        actor.transform.localPosition = (offsetRight + offsetRight2 * actor.transform.parent.GetSiblingIndex()) * step;
                    }

                }

                if (_actor.controlAI)
                {
                    _actor.transform.localPosition = (offsetLeft + offsetLeft2 * actor.transform.parent.GetSiblingIndex()) * step;
                }
                else
                {
                    _actor.transform.localPosition = (offsetRight + offsetRight2 * actor.transform.parent.GetSiblingIndex()) * step;
                }

                yield return null;
            }

            layer = LayerMask.NameToLayer("Characters");
            foreach (var pair in _onDamage)
            {
                var actor = pair.Key;
                actor.gameObject.layer = layer;
                actor.transform.localPosition = Vector3.zero;
            }

            _actor.gameObject.layer = layer;
            _actor.transform.localPosition = Vector3.zero;
        }

        private void GroupCallback(Group sender)
        {
            _watch = false;
        }

        private void OnWinCallback()
        {
            if (OnWin != null)
            {
                OnWin(this);
            }
        }

        private void OnLoseCallback()
        {
            if (OnLose != null)
            {
                OnLose(this);
            }
        }

        private void OnRoundCallback()
        {
            ++_roundNumber;

            if (OnRound != null)
            {
                OnRound(this);
            }
        }

        private void OnStageCallback()
        {
            _actionStack.Clear();
            _onDamage.Clear();
            _watchProcces = null;

            if (OnStage != null)
            {
                OnStage(this);
            }
        }

        private void StageHandler(Battle sender)
        {
            var actor = sender.actor;
            if (actor.controlAI)
            {
                Group target = null;

                var skill = actor.puppet.skillSet[0][0];
                if (skill.goal == SkillGoal.Enemies)
                {
                    target = sender.heroes;
                }
                else
                {
                    target = sender.enemies;
                }

                var goals = new List<int>(4);
                for (var i = 0; i < 4; ++i)
                {
                    if (IsPositionForTarget(skill.position, i + 1) && (i == Mathf.Clamp(i, 0, target.count - 1)))
                    {
                        goals.Add(i);
                    }
                }

                sender.AddAction(new Action(skill, target[goals[Random.Range(0, goals.Count)]]));
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
    }
    	
}