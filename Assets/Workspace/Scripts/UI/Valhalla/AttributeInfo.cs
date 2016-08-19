// <copyright file="AttributeInfo.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEngine.UI;

using V4F.Character;

namespace V4F.UI.Valhalla
{

    public class AttributeInfo : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private Statistics _stats;

        [SerializeField, HideInInspector]
        private AttributeType _type;

        [SerializeField, HideInInspector]
        private Text _titleUI;

        [SerializeField, HideInInspector]
        private Text _valueUI;

        [SerializeField, HideInInspector]
        private string _title;

        private void OnHeroFocused(Actor hero)
        {
            _valueUI.text = string.Format("{0}", hero[_type].value);
        }

        private void Start()
        {
            _titleUI.text = _title;
            _valueUI.text = "";
        }

        private void OnEnable()
        {            
            _stats.OnHeroFocused += OnHeroFocused;
        }

        private void OnDisable()
        {
            _stats.OnHeroFocused -= OnHeroFocused;
        }
    }
	
}
