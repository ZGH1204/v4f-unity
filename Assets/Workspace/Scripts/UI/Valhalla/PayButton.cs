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

        [Range(0f, 10f)]
        public float speedFade = 1f;

        private IEnumerator _fadeProcess = null;
        private float _alpha = 1f;
        private float _factor = 1f;

        protected override void Awake()
        {
            base.Awake();

            _alpha = image.canvasRenderer.GetAlpha();
        }

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
            locked = true;
            PlayFade(locked);
        }

        private void OnPageViewScrollEnd(PageView sender, PageViewEventArgs args)
        {
            locked = false;
            PlayFade(locked);            
        }

        private void PlayFade(bool hide)
        {
            _factor = (hide ? -1f : 1f);

            if (_fadeProcess == null)
            {
                _fadeProcess = Fade();
                StartCoroutine(_fadeProcess);
            }
        }

        private void StopFade()
        {
            if (_fadeProcess != null)
            {
                StopCoroutine(_fadeProcess);
                _fadeProcess = null;
            }
        }

        private IEnumerator Fade()
        {
            _alpha = Mathf.Clamp01(_alpha + speedFade * _factor * Time.deltaTime);

            while ((_alpha > 0f) && (_alpha < 1f))
            {                                
                image.canvasRenderer.SetAlpha(_alpha);

                yield return null;

                _alpha = Mathf.Clamp01(_alpha + speedFade * _factor * Time.deltaTime);
            }

            _alpha = Mathf.Clamp01(_alpha);
            image.canvasRenderer.SetAlpha(_alpha);

            _fadeProcess = null;
        }
    }
	
}
