// <copyright file="ITool.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

namespace V4F.MapEditor
{

    public interface ITool
    {
        void Enable(Form sender);

        void Disable(Form sender);

        bool IsTool(Tools tool);

        bool OnMouseDown(Form sender, ModKey key, int button);

        bool OnMouseUp(Form sender, ModKey key, int button);

        bool OnMouseDrag(Form sender, ModKey key, int button);

        bool OnMouseMove(Form sender, ModKey key);
    }
	
}
