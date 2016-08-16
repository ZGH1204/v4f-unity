// <copyright file="Skill.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Character
{

    [System.Serializable]
    public class Skill : UniqueObject
    {
        #region Constants
        public const int AllStages = 5;
        #endregion

        #region Fields
        [SerializeField, HideInInspector]
        private SkillStage[] _stages = null;
        #endregion

        #region Properties
        public SkillStage this[int stage]
        {
            get { return _stages[stage]; }
        }
        #endregion

        #region Methods
        public void Initialize()
        {
            _stages = new SkillStage[AllStages];
            for (var i = 0; i < AllStages; ++i)
            {
                _stages[i] = new SkillStage();
            }
        }

        private void Reset()
        {
            Initialize();
        }
        #endregion
    }

}
