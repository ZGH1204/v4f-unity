// <copyright file="PuppetSpec.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Puppets
{

    public enum Stats
    {
        HealthPoints,
        Accuracy,
        Initiative,
        Stamina,
        MinDamage,
        MaxDamage,
        CriticalChance,
        CriticalDamage,
    }

    [System.Serializable]
    public class PuppetSpec : ScriptableObject
    {
        #region Constants
        public const int StatsCapacity = 32;
        #endregion

        #region Fields
        [SerializeField, HideInInspector]
        private int[] _stats;        
        #endregion

        #region Properties

        #endregion

        #region Methods
        public void Initialize()
        {
            _stats = new int[StatsCapacity];
            _stats[(int)Stats.HealthPoints] = 1;
            _stats[(int)Stats.Accuracy] = 1;
            _stats[(int)Stats.Initiative] = 1;
            _stats[(int)Stats.Stamina] = 1;
            _stats[(int)Stats.MinDamage] = 1;
            _stats[(int)Stats.MaxDamage] = 1;
            _stats[(int)Stats.CriticalChance] = 0;
            _stats[(int)Stats.CriticalDamage] = 0;
        }

        public int GetStat(Stats stat)
        {
            return _stats[stat.GetIndex()];
        }

        public void SetStat(Stats stat, int value)
        {
            _stats[stat.GetIndex()] = value;
        }
        #endregion
    }

}
