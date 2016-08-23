// <copyright file="SkillSet.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

namespace V4F.Character
{

    [System.Serializable]
    public class SkillSet : ScriptableObject
    {
        #region Constants
        public const int MaxSkills = 4;
        #endregion

        #region Fields
        [SerializeField, HideInInspector]
        private List<Skill> _skills = null;
        #endregion

        #region Properties
        public int countSkills
        {
            get { return _skills.Count; }
        }

        public Skill this[int index]
        {
            get { return _skills[index]; }
        }
        #endregion

        #region Methods
        private void OnEnable()
        {
            if (_skills == null)
            {
                _skills = new List<Skill>(MaxSkills);
            }
        }
        #endregion
    }

}
