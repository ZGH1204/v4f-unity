// <copyright file="AttributeInfo.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEngine.UI;

using V4F.Character;

namespace V4F.UI.Valhalla
{

    public class AttributeInfo : MonoBehaviour
    {
        private const string _Format01 = "{0} ";
        private const string _Format02 = "{0}%";

        [SerializeField, HideInInspector]
        private Dispatcher _dispatcher;

        [SerializeField, HideInInspector]
        private AttributeType _type;

        [SerializeField, HideInInspector]
        private Text _titleUI;

        [SerializeField, HideInInspector]
        private Text _valueUI;

        [SerializeField, HideInInspector]
        private string _title;

        [SerializeField, HideInInspector]
        private bool _percent;

        private void OnHeroSelected(Actor hero)
        {
            _valueUI.text = string.Format((_percent ? _Format02 : _Format01), hero[_type].value);
        }

        private void Start()
        {
            _titleUI.text = _title;
        }

        private void OnEnable()
        {
            _dispatcher.OnHeroSelected += OnHeroSelected;
        }

        private void OnDisable()
        {
            _dispatcher.OnHeroSelected -= OnHeroSelected;
        }
    }
	
}
