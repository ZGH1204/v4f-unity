// <copyright file="BaseTool.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

namespace V4F.MapEditor
{

    public class BaseTool : ITool
    {
        private Tools _tool = Tools.None;        

        public BaseTool(Tools tool)
        {
            _tool = tool;
        }

        public virtual void Enable(Form sender)
        {
            sender.hoverEnabled = false;
            sender.closestEnabled = false;
        }

        public virtual void Disable(Form sender)
        {

        }

        public bool IsTool(Tools tool)
        {
            return (_tool == tool);
        }

        public virtual bool OnMouseDown(Form sender, ModKey key, int button)
        {
            return false;
        }

        public virtual bool OnMouseUp(Form sender, ModKey key, int button)
        {
            return false;
        }

        public virtual bool OnMouseDrag(Form sender, ModKey key, int button)
        {
            return false;
        }

        public virtual bool OnMouseMove(Form sender, ModKey key)
        {
            return false;
        }        
    }

}
