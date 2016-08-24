// <copyright file="IListItem.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.UI
{

    public interface IListItem
    {
        RectTransform rect { get; }
        Vector2 size { get; set; }        
	    bool select { get; set; }

        void Clear();
    }
	
}
