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
        private static readonly int __anim_param_attack = Animator.StringToHash("attack");
        private static readonly int __anim_param_damage = Animator.StringToHash("damage");

        public delegate void BattleEventHandler(Battle sender);

        public static event BattleEventHandler OnStart;
        public static event BattleEventHandler OnFocusGroup;
        public static event BattleEventHandler OnUnfocusGroup;
        public static event BattleEventHandler OnEnd;

        public event BattleEventHandler OnWin;
        public event BattleEventHandler OnLose;
        public event BattleEventHandler OnRound;
        public event BattleEventHandler OnStage;
        public event BattleEventHandler OnStageFinish;

        public Party heroes;
        public Camera zoom;
        public CanvasGroup zoomFade;
        public GameObject healthUIRoot;

        public Transform leftSide;
        public Transform rightSide;

        public StateTransition transition;
        public State state;
        
        private Squad _enemies;

        private List<Actor> _queueHeroes;
        private List<Actor> _queueEnemies;
        private Group _group;
        private Actor _actor;
        private bool _lastAI;
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

        private static void OnStartCallback(Battle sender)
        {
            if (OnStart != null)
            {
                OnStart(sender);
            }
        }

        private static void OnFocusGroupCallback(Battle sender)
        {
            if (OnFocusGroup != null)
            {
                OnFocusGroup(sender);
            }
        }

        private static void OnUnfocusGroupCallback(Battle sender)
        {
            if (OnUnfocusGroup != null)
            {
                OnUnfocusGroup(sender);
            }
        }

        private static void OnEndCallback(Battle sender)
        {
            if (OnEnd != null)
            {
                OnEnd(sender);
            }
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
            OnStartCallback(this);

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
                        OnFocusGroupCallback(this);

                        var timer = (_lastAI != _actor.controlAI) ? 1.5f : 0.5f;
                        _lastAI = _actor.controlAI;
                        while (timer > 0f)
                        {
                            timer -= Time.deltaTime;
                            yield return null;
                        }

                        OnStageCallback();
                        while (!(_actionStack.Count > 0))
                        {
                            yield return null;
                        }

                        OnStageFinishCallback();
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

            OnEndCallback(this);

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
            Vector3 defFinish = Vector3.zero;
            Vector3 defStart = Vector3.zero;

            var layer = LayerMask.NameToLayer("Zoom");
            foreach (var pair in _onDamage)
            {
                var actor = pair.Key;
                actor.gameObject.layer = layer;
                if (pair.Value != -1)
                {
                    actor.animator.SetBool(__anim_param_damage, true);
                    defFinish = (actor.controlAI ? rightSide : leftSide).position;
                    defStart = actor.transform.position;
                }
            }

            _actor.gameObject.layer = layer;
            _actor.animator.SetBool(__anim_param_attack, true);

            healthUIRoot.SetActive(false);

            OnUnfocusGroupCallback(this);

            zoomFade.alpha = 0f;
            zoom.gameObject.SetActive(true);

            var attackFinish = (_actor.controlAI ? rightSide : leftSide).position;
            var attackStart = _actor.transform.position;

            while (zoom.fieldOfView > 40f)
            {                
                zoom.fieldOfView -= Time.deltaTime * 75f;
                var step = Mathf.Clamp01((50f - zoom.fieldOfView) * 0.1f);

                zoomFade.alpha = 0.8f * step;

                foreach (var pair in _onDamage)
                {
                    var actor = pair.Key;
                    actor.transform.position = Vector3.Lerp(defStart, defFinish, step);
                }                

                _actor.transform.position = Vector3.Lerp(attackStart, attackFinish, step);

                yield return null;
            }

            zoom.fieldOfView = 40f;

            yield return new WaitForSeconds(1.2f);
            
            foreach (var pair in _onDamage)
            {
                var actor = pair.Key;
                actor.animator.SetBool(__anim_param_damage, false);                
            }

            _actor.animator.SetBool(__anim_param_attack, false);
            var temp = attackStart;
            attackStart = attackFinish;
            attackFinish = temp;

            temp = defStart;
            defStart = defFinish;
            defFinish = temp;

            while (zoom.fieldOfView < 50f)
            {
                zoom.fieldOfView += Time.deltaTime * 75f;
                var step = Mathf.Clamp01((50f - zoom.fieldOfView) * 0.1f);

                zoomFade.alpha = 0.8f * step;

                foreach (var pair in _onDamage)
                {
                    var actor = pair.Key;
                    actor.transform.position = Vector3.Lerp(defStart, defFinish, step);
                }                

                _actor.transform.position = Vector3.Lerp(attackStart, attackFinish, step);

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

            zoom.fieldOfView = 50f;
            zoom.gameObject.SetActive(false);

            healthUIRoot.SetActive(true);
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

        private void OnStageFinishCallback()
        {            
            if (OnStageFinish != null)
            {
                OnStageFinish(this);
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