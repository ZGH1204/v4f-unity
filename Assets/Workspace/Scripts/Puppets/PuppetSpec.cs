// <copyright file="PuppetSpec.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Puppets
{

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
            _stats[(int)PuppetStats.HealthPoints] = 1;
            _stats[(int)PuppetStats.Accuracy] = 1;
            _stats[(int)PuppetStats.Initiative] = 1;
            _stats[(int)PuppetStats.Stamina] = 1;
            _stats[(int)PuppetStats.MinDamage] = 1;
            _stats[(int)PuppetStats.MaxDamage] = 1;
            _stats[(int)PuppetStats.CriticalChance] = 0;
            _stats[(int)PuppetStats.CriticalDamage] = 0;
        }

        public int GetStat(PuppetStats stat)
        {
            return _stats[stat.GetIndex()];
        }

        public void SetStat(PuppetStats stat, int value)
        {
            _stats[stat.GetIndex()] = value;
        }
        #endregion
    }

}
