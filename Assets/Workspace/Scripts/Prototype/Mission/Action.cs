// <copyright file="Action.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using V4F.Character;

namespace V4F.Prototype.Mission
{

    public class Action
    {        
        public SkillStage skill { get; private set; }
        public Actor goal { get; private set; }

        public Action(SkillStage skill, Actor goal)
        {
            this.skill = skill;            
            this.goal = goal;
        }
    }
	
}
