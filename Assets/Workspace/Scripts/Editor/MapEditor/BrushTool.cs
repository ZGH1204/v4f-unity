// <copyright file="BrushTool.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

namespace V4F.MapEditor
{

    public class BrushTool : BaseTool
    {
        private bool _capture;

        public BrushTool(Tools tool) : base(tool)
        {
            _capture = false;
        }

        public override void Enable(Form sender)
        {
            sender.hoverRoomEnabled = true;
            sender.hoverTransitionEnabled = false;
            sender.closestEnabled = true;
        }

        public override bool OnMouseDown(Form sender, ModKey key, int button)
        {
            if (!_capture && (button == 0))
            {
                sender.TryActivateHall();
                _capture = true;

                return true;
            }

            return false;
        }

        public override bool OnMouseUp(Form sender, ModKey key, int button)
        {
            if (_capture && (button == 0))
            {
                _capture = false;
                return true;
            }

            return false;
        }


    }
	
}
