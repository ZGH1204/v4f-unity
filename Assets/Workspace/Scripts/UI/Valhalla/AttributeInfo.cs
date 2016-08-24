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
        private const string _Format03 = "- {0} ";
        private const string _Format04 = "- {0}%";

        [SerializeField, HideInInspector]
        private Dispatcher _dispatcher;

        [SerializeField, HideInInspector]
        private AttributeType _type;

        [SerializeField, HideInInspector]
        private AttributeType _type2;

        [SerializeField, HideInInspector]
        private Text _titleUI;

        [SerializeField, HideInInspector]
        private Text _valueUI;

        [SerializeField, HideInInspector]
        private Text _valueUI2;

        [SerializeField, HideInInspector]
        private string _title;

        [SerializeField, HideInInspector]
        private bool _percent;

        [SerializeField, HideInInspector]
        private bool _percent2;

        private void OnHeroSelected(Actor hero)
        {
            if (_valueUI != null)
            {
                _valueUI.text = string.Format((_percent ? _Format02 : _Format01), hero[_type].value);
            }                
            if (_valueUI2 != null)
            {
                _valueUI2.text = string.Format((_percent2 ? _Format04 : _Format03), hero[_type2].value);
            }
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
