// <copyright file="Database.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

using V4F.Puppets;

namespace V4F.Game
{
        
    public class Database
    {
        #region Fields
        private static Database __instance = null;

        private Dictionary<string, Hero> _heroesTable = null;
        private List<Hero> _heroesList = null;
        #endregion

        #region Properties

        #endregion

        #region Constructors
        private Database()
        {
            _heroesTable = new Dictionary<string, Hero>(256);
            _heroesList = new List<Hero>(256);
        }
        #endregion

        #region Methods        
        public static Hero[] QueryHeroesByLocation(Location location)
        {
            return __instance._heroesList.FindAll(x => x.location == location).ToArray();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            __instance = new Database();

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
                var hero = new Hero(puppet, Location.Draft);
                _heroesTable.Add(puppet.uniqueID, hero);
                _heroesList.Add(hero);
            }            
        }        
        #endregion
    }

}
