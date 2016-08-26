// <copyright file="AvatarUI.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

using V4F.Character;
using V4F.Game;

namespace V4F.UI.Valhalla
{

    public class AvatarUI : MonoBehaviour
    {        
        public Dispatcher dispatcher;

        private GameObject _avatar = null;

        private void OnHeroSelected(Actor hero)
        {
            if (_avatar != null)
            {
                DestroyObject(_avatar);
                _avatar = null;
            }

            // Добавить буферизацию объектов UI, если понадобится!

            _avatar = Instantiate(hero.puppet.prefabUI, Vector3.zero, Quaternion.identity) as GameObject;
            _avatar.transform.localScale = Vector3.one;
            _avatar.transform.SetParent(transform, false);
        }

        private void OnEnable()
        {
            dispatcher.OnHeroSelected += OnHeroSelected;
        }

        private void OnDisable()
        {
            dispatcher.OnHeroSelected -= OnHeroSelected;
        }
    }
	
}
