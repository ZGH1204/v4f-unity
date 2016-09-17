// <copyright file="Hero.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

using V4F.Game;

namespace V4F.Character
{

    public class Hero : Actor
    {
        public delegate void HeroEventsHandler(Hero sender);

        public event HeroEventsHandler OnDeath;

        private Actor _self = null;

        public Hero(Actor actor) : base(actor.puppet)
        {
            _self = actor;
        }

        public override bool TakeDamage(int value)
        {
            var result = base.TakeDamage(value);

            if (result)
            {
                _self.location = Location.Valhalla;
                _self = null;

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
