// <copyright file="Skill.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;

using UnityEngine;

namespace V4F.Character
{

    [System.Serializable]
    public class Skill : UniqueObject
    {
        #region Constants
        public const int AllStages = 3;
        #endregion

        #region Fields
        [SerializeField, HideInInspector]
        private List<SkillStage> _stages = null;

        [SerializeField, HideInInspector]
        private int _lastEditStageIndex = -1;
        #endregion

        #region Properties
        public SkillStage this[int stage]
        {
            get { return _stages[stage]; }
        }

        public int lastEditStageIndex
        {
            get { return _lastEditStageIndex; }
            set { _lastEditStageIndex = value; }
        }

        public bool editCompleted
        {
            get
            {
                var completed = true;
                for (var i = 0; (i < AllStages) && completed; ++i)
                {
                    completed = completed && _stages[i].validate;
                }
                return completed;
            }
        }
        #endregion

        #region Methods
        protected override void OnEnable()
        {
            base.OnEnable();

            if (_stages == null)
            {
                _stages = new List<SkillStage>(AllStages);                
            }
        }
        #endregion
    }

}
