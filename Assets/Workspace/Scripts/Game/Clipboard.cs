// <copyright file="Clipboard.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

namespace V4F.Game
{

    public class Clipboard
    {
        #region Fields
        private static Clipboard __instance = null;

        private Stack<Hero> _heroes = null;
        #endregion

        #region Constructors
        private Clipboard()
        {
            _heroes = new Stack<Hero>(16);
        }
        #endregion

        #region Methods
        public static void SetHero(Hero hero)
        {
            var buffer = __instance._heroes;
            if (hero != null)
            {
                buffer.Push(hero);
            }            
        }

        public static Hero GetHero()
        {
            var buffer = __instance._heroes;
            if (buffer.Count > 0)
            {
                return buffer.Pop();
            }
            return null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            __instance = new Clipboard();
        }        
        #endregion
    }

}
