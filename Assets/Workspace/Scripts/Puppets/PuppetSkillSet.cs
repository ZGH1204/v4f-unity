// <copyright file="PuppetSkillSet.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

namespace V4F.Puppets
{

    [System.Serializable]
    public class PuppetSkillSet : ScriptableObject
    {
        #region Constants
        public const int MaxSkills = 4;
        #endregion

        #region Fields
        [SerializeField, HideInInspector]
        private List<PuppetSkill> _skills = new List<PuppetSkill>(MaxSkills);
        #endregion

        #region Properties
        public int countSkills
        {
            get { return _skills.Count; }
        }

        public PuppetSkill this[int index]
        {
            get { return _skills[index]; }
        }
        #endregion
    }

}
