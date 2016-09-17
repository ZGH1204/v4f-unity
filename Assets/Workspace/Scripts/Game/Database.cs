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
        private Dictionary<string, Actor> _enemiesTable = null;
        private List<Actor> _enemiesList = null;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        private Database(string save)
        {
            _heroesTable = new Dictionary<string, Actor>(256);
            _heroesList = new List<Actor>(256);
            _enemiesTable = new Dictionary<string, Actor>(256);
            _enemiesList = new List<Actor>(256);
        }
        #endregion

        #region Methods        
        public static Actor[] QueryHeroesByLocation(Location location)
        {
            return __instance._heroesList.FindAll(x => x.location == location).ToArray();
        }

        public static Actor[] QueryRandomEnemies(int count)
        {
            var enemies = __instance._enemiesList;
            var result = new Actor[count];
            var all = enemies.Count;

            for (var i = 0; i < count; ++i)
            {
                result[i] = enemies[Random.Range(0, all)];
            }

            return result;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            __instance = new Database("SaveFile.sav"); // пока просто образец

            var storages = Resources.LoadAll<PuppetStorage>("Heroes");
            for (var i = 0; i < storages.Length; ++i)
            {
                __instance.IncludeHeroes(storages[i]);
            }

            storages = Resources.LoadAll<PuppetStorage>("Enemies");
            for (var i = 0; i < storages.Length; ++i)
            {
                __instance.IncludeEnemies(storages[i]);
            }
        }
        
        private void IncludeHeroes(PuppetStorage storage)
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
                actor.controlAI = false;

                _heroesTable.Add(puppet.uniqueID, actor);
                _heroesList.Add(actor);
            }            
        }

        private void IncludeEnemies(PuppetStorage storage)
        {
            var count = storage.countPuppets;
            for (var i = 0; i < count; ++i)
            {
                var puppet = storage[i];

                var actor = new Actor(puppet);
                // load data form save for actor
                // ...
                // while
                actor.location = Location.None;
                actor.controlAI = true;

                _enemiesTable.Add(puppet.uniqueID, actor);
                _enemiesList.Add(actor);
            }
        }
        #endregion
    }

}
