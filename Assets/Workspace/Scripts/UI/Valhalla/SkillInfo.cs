// <copyright file="SkillsList.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEngine.UI;

namespace V4F.UI.Valhalla
{

    public class SkillInfo : MonoBehaviour
    {
        public SkillsList skills;
        public Text info;

        private void OnEnable()
        {
            skills.OnSelect += OnSkillSelect;
        }

        private void OnDisable()
        {
            skills.OnSelect -= OnSkillSelect;
        }

        private void OnSkillSelect(ListBox<SkillItem> sender, ListBoxEventArgs args)
        {
            var item = sender[args.index];
            var skill = item.skill;
            info.text = skill[0].description;
        }

    }
	
}
