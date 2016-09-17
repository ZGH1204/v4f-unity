// <copyright file="BattlePanelHandler.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using V4F.Character;
using V4F.Prototype.Mission;
using V4F.Prototype.Map;

namespace V4F.UI.Mission
{

    public class BattlePanelHandler : MonoBehaviour
    {
        public delegate void SelectSkillHandler(int number);

        public event SelectSkillHandler OnSelectSkill;

        public Button skill1;
        public Button skill2;
        public Button skill3;
        public Button skill4;

        private Button _lastButton;
        private int _skillNumber;

        public bool locked
        {            
            set
            {
                if (value)
                {
                    skill1.disable = true;
                    skill1.transform.GetChild(0).gameObject.SetActive(false);
                    skill2.disable = true;
                    skill2.transform.GetChild(0).gameObject.SetActive(false);
                    skill3.disable = true;
                    skill3.transform.GetChild(0).gameObject.SetActive(false);
                    skill4.disable = true;
                    skill4.transform.GetChild(0).gameObject.SetActive(false);
                }
                else
                {
                    skill1.disable = false;
                    skill1.transform.GetChild(0).gameObject.SetActive(false);
                    skill2.disable = false;
                    skill2.transform.GetChild(0).gameObject.SetActive(false);
                    skill3.disable = false;
                    skill3.transform.GetChild(0).gameObject.SetActive(false);
                    skill4.disable = false;
                    skill4.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }

        public int skillNumber
        {
            get { return _skillNumber; }
            private set
            {
                _skillNumber = value;
                OnSelectSkillCallback(_skillNumber);
            }
        }
	
        public void SetActor(Actor actor)
        {
            if (actor != null)
            {
                var skillset = actor.puppet.skillSet;
                var index = -1;

                skill1.transform.GetChild(0).gameObject.SetActive(false);
                if (++index < skillset.countSkills)
                {
                    var skill = skillset[index][0];
                    skill1.normal = skill.icon;
                    skill1.pressed = skill.icon;
                    skill1.Refresh();
                    skill1.locked = false;
                }
                else skill1.disable = true;

                skill2.transform.GetChild(0).gameObject.SetActive(false);
                if (++index < skillset.countSkills)
                {
                    var skill = skillset[index][0];
                    skill2.normal = skill.icon;
                    skill2.pressed = skill.icon;
                    skill2.Refresh();
                    skill2.locked = false;
                }
                else skill2.disable = true;

                skill3.transform.GetChild(0).gameObject.SetActive(false);
                if (++index < skillset.countSkills)
                {
                    var skill = skillset[index][0];
                    skill3.normal = skill.icon;
                    skill3.pressed = skill.icon;
                    skill3.Refresh();
                    skill3.locked = false;
                }
                else skill3.disable = true;

                skill4.transform.GetChild(0).gameObject.SetActive(false);
                if (++index < skillset.countSkills)
                {
                    var skill = skillset[index][0];
                    skill4.normal = skill.icon;
                    skill4.pressed = skill.icon;
                    skill4.Refresh();
                    skill4.locked = false;
                }
                else skill4.disable = true;
            }
            else
            {
                skill1.disable = true;
                skill2.disable = true;
                skill3.disable = true;
                skill4.disable = true;
            }

            skillNumber = -1;
        }

        private void OnSelectSkillCallback(int number)
        {
            if (OnSelectSkill != null)
            {
                OnSelectSkill(number);
            }
        }

        private void Skill1ClickEventHandler(Button sender, ButtonEventArgs args)
        {
            if (_lastButton != null)
            {
                _lastButton.locked = false;
                _lastButton.transform.GetChild(0).gameObject.SetActive(false);
            }

            skillNumber = 0;
            sender.locked = true;
            _lastButton = sender;
            _lastButton.transform.GetChild(0).gameObject.SetActive(true);
        }

        private void Skill2ClickEventHandler(Button sender, ButtonEventArgs args)
        {
            if (_lastButton != null)
            {
                _lastButton.locked = false;
                _lastButton.transform.GetChild(0).gameObject.SetActive(false);
            }

            skillNumber = 1;
            sender.locked = true;
            _lastButton = sender;
            _lastButton.transform.GetChild(0).gameObject.SetActive(true);
        }

        private void Skill3ClickEventHandler(Button sender, ButtonEventArgs args)
        {
            if (_lastButton != null)
            {
                _lastButton.locked = false;
                _lastButton.transform.GetChild(0).gameObject.SetActive(false);
            }

            skillNumber = 2;
            sender.locked = true;
            _lastButton = sender;
            _lastButton.transform.GetChild(0).gameObject.SetActive(true);
        }

        private void Skill4ClickEventHandler(Button sender, ButtonEventArgs args)
        {
            if (_lastButton != null)
            {
                _lastButton.locked = false;
                _lastButton.transform.GetChild(0).gameObject.SetActive(false);
            }

            skillNumber = 3;
            sender.locked = true;
            _lastButton = sender;
            _lastButton.transform.GetChild(0).gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            skill1.OnClick += Skill1ClickEventHandler;
            skill2.OnClick += Skill2ClickEventHandler;
            skill3.OnClick += Skill3ClickEventHandler;
            skill4.OnClick += Skill4ClickEventHandler;
        }

        private void OnDisable()
        {
            skill1.OnClick -= Skill1ClickEventHandler;
            skill2.OnClick -= Skill2ClickEventHandler;
            skill3.OnClick -= Skill3ClickEventHandler;
            skill4.OnClick -= Skill4ClickEventHandler;
        }

    }
	
}
