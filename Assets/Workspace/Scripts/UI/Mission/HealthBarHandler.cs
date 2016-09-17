// <copyright file="HealthBarHandler.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using V4F.Prototype.Mission;
using V4F.Prototype.Map;

namespace V4F.UI.Mission
{

    [RequireComponent(typeof(Slider))]
    public class HealthBarHandler : MonoBehaviour
    {
        private RectTransform _rect = null;
        private Slider _slider = null;
        private Text _text = null;
        private int _current = 0;
        private int _maximum = 0;

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void SetPosition(Vector2 position)
        {
            _rect.anchoredPosition = position;
        }

        public void SetValue(int current, int maximum)
        {
            var changed = (_current != current) || (_maximum != maximum);
            if (changed)
            {
                _current = current;
                _maximum = maximum;
                _text.text = string.Format("{0}/{1}", _current, _maximum);
                _slider.minValue = 0;
                _slider.maxValue = _maximum;
                _slider.value = _current;
            }
        }

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _slider = GetComponent<Slider>();
            _text = GetComponentInChildren<Text>();
        }
    }
	
}
