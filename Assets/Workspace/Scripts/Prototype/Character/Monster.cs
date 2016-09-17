// <copyright file="Monster.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

using V4F.Game;

namespace V4F.Character
{

    public class Monster : Actor
    {
        public delegate void MonsterEventsHandler(Monster sender);

        public event MonsterEventsHandler OnDeath;

        public Monster(Puppet puppet) : base(puppet)
        {

        }

        public override bool TakeDamage(int value)
        {
            var result = base.TakeDamage(value);

            if (result)
            {
                OnDeathCallback();
            }

            return result;
        }

        private void OnDeathCallback()
        {
            if (OnDeath != null)
            {
                OnDeath(this);
            }
        }
    }
	
}
