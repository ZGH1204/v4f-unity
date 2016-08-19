// <copyright file="Clipboard.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

using V4F.Character;

namespace V4F.Game
{

    public class Clipboard
    {
        #region Fields
        private static Clipboard __instance = null;
        private Stack<Actor> _actors = null;
        #endregion

        #region Constructors
        private Clipboard()
        {
            _actors = new Stack<Actor>(16);
        }
        #endregion

        #region Methods
        public static void SetActor(Actor actor)
        {
            var buffer = __instance._actors;
            if (actor != null)
            {
                buffer.Push(actor);
            }            
        }

        public static Actor GetActor()
        {
            var buffer = __instance._actors;
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
