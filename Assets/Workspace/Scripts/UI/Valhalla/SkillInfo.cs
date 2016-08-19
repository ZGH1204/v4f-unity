// <copyright file="SkillInfo.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;
using UnityEngine.UI;

using V4F.Character;

namespace V4F.UI.Valhalla
{

    public class SkillInfo : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        private Statistics _stats;                

        [SerializeField, HideInInspector]
        private Text _valueUI;        

        private void OnSkillFocusedCallback(Skill skill)
        {
            if (skill != null)
            {
                _valueUI.text = string.Format("{0}", skill[0].description);
            }
            else
            {
                _valueUI.text = "";
            }
        }

        private void Start()
        {
            _valueUI.text = "";
        }

        private void OnEnable()
        {
            _stats.OnSkillFocused += OnSkillFocusedCallback;
        }

        private void OnDisable()
        {
            _stats.OnSkillFocused -= OnSkillFocusedCallback;
        }
    }

}
