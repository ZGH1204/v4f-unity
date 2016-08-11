// <copyright file="ICell.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.UI
{

    public interface ICell<T>
    {
        #region Properties
        RectTransform rect
        {
            get;
        }

        T subject
        {
            get;
            set;
        }

        bool empty
        {
            get;
        }
        #endregion
    }

}
