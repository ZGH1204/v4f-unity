// <copyright file="Database.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

using V4F.Character;

namespace V4F.Game
{
        
    public class Database
    {
        #region Fields
        private static Database __instance = null;
        private Dictionary<string, Actor> _heroesTable = null;
        private List<Actor> _heroesList = null;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        private Database(string save)
        {
            _heroesTable = new Dictionary<string, Actor>(256);
            _heroesList = new List<Actor>(256);
        }
        #endregion

        #region Methods        
        public static Actor[] QueryHeroesByLocation(Location location)
        {
            return __instance._heroesList.FindAll(x => x.location == location).ToArray();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            __instance = new Database("SaveFile.sav"); // пока просто образец

            var storages = Resources.LoadAll<PuppetStorage>("");
            for (var i = 0; i < storages.Length; ++i)
            {
                __instance.Include(storages[i]);
            }            
        }
        
        private void Include(PuppetStorage storage)
        {
            var count = storage.countPuppets;
            for (var i = 0; i < count; ++i)
            {
                var puppet = storage[i];

                var actor = new Actor(puppet);
                // load data form save for actor
                // ...
                // while
                actor.location = Location.Valhalla;

                _heroesTable.Add(puppet.uniqueID, actor);
                _heroesList.Add(actor);
            }            
        }        
        #endregion
    }

}
