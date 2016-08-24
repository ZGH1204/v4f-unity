// <copyright file="SkillsList.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

using V4F.Character;

namespace V4F.UI.Valhalla
{

    public class SkillsList : ListBox<SkillItem>
    {
        public SkillItem itemPrefab;
        public Dispatcher dispatcher;

        protected override SkillItem prefab
        {
            get { return itemPrefab; }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            dispatcher.OnHeroSelected += OnHeroSelected;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            dispatcher.OnHeroSelected += OnHeroSelected;
        }

        private void OnHeroSelected(Actor hero)
        {
            Clear();

            var skillset = hero.puppet.skillSet;
            SkillItem item = null;

            for (var i = 0; i < skillset.countSkills; ++i)
            {
                AddItem(out item);
                item.skill = skillset[i];
            }

            SelectItem(0);
        }
    }

}
