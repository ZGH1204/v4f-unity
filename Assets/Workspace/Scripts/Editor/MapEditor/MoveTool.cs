// <copyright file="MoveTool.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.MapEditor
{

    public class MoveTool : BaseTool
    {
        private Vector3 _capturePosition;
        private Vector2 _captureOffset;
        private int _captureButton;
        private bool _capture;

        public MoveTool(Tools tool) : base(tool)
        {
            _captureButton = -1;
            _capture = false;
        }

        public override bool OnMouseDown(Form sender, ModKey key, int button)
        {            
            if (!_capture && ((button == 0) || (button == 2)))
            {
                _capturePosition = sender.mousePosition;
                _captureOffset = sender.displayOffset;
                _captureButton = button;
                _capture = true;

                return true;
            }            

            return false;
        }

        public override bool OnMouseDrag(Form sender, ModKey key, int button)
        {
            if (_capture && (_captureButton == button))
            {
                Vector2 delta = sender.mousePosition - _capturePosition;
                sender.displayOffset = _captureOffset + delta;

                return true;
            }

            return false;      
        }

        public override bool OnMouseUp(Form sender, ModKey key, int button)
        {            
            if (_capture && (_captureButton == button))
            {
                _captureButton = -1;
                _capture = false;

                return true;
            }

            return false;
        }
    }

}
