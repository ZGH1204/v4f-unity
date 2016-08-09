// <copyright file="IDraggable.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.UI
{

    public interface IDraggable
    {
        #region Properties
        DraggableType type
        {
            get;
        }

        Sprite icon
        {
            get;
        }
        #endregion
    }

}
