// <copyright file="PayButton.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

using V4F.UI;

namespace V4F.UI.Valhalla
{

    public class PayButton : Button
    {
        public PageView pageView;        

        protected override void OnEnable()
        {
            base.OnEnable();

            pageView.OnScrollBegin += OnPageViewScrollBegin;
            pageView.OnScrollEnd += OnPageViewScrollEnd;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            pageView.OnScrollBegin -= OnPageViewScrollBegin;
            pageView.OnScrollEnd -= OnPageViewScrollEnd;
        }

        private void OnPageViewScrollBegin(PageView sender, PageViewEventArgs args)
        {
            disable = true;
        }

        private void OnPageViewScrollEnd(PageView sender, PageViewEventArgs args)
        {
            disable = false;
        }        
    }
	
}
