// <copyright file="Dispatcher.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

using V4F.Character;

namespace V4F.UI.Valhalla
{

    public class Dispatcher : MonoBehaviour
    {
        public delegate void HeroSelectedEventHandler(Actor hero);

        public event HeroSelectedEventHandler OnHeroSelected;

        public ValhallaList valhalla;
        public BarracksList barracks;
        public GameObject GUI;

        private ValhallaItem _selectOnValhalla = null;
        private ValhallaItem _selectOnBarracks = null;
        private Actor _selectActor = null;

        private Actor selectActor
        {
            get { return _selectActor; }
            set
            {
                _selectActor = value;
                if (_selectActor != null)
                {
                    if (!GUI.activeInHierarchy)
                    {
                        GUI.SetActive(true);
                    }

                    if (OnHeroSelected != null)
                    {
                        OnHeroSelected(_selectActor);
                    }                        
                }
            }
        }

        private void OnValhallaSelect(ListBox<ValhallaItem> sender, ListBoxEventArgs args)
        {
            if (_selectOnBarracks != null)
            {
                barracks.ResetSelection();
                _selectOnBarracks = null;
            }

            _selectOnValhalla = sender[args.index];
            selectActor = _selectOnValhalla.actor;
        }

        private void OnBarracksSelect(ListBox<ValhallaItem> sender, ListBoxEventArgs args)
        {
            if (_selectOnValhalla != null)
            {
                valhalla.ResetSelection();
                _selectOnValhalla = null;
            }

            _selectOnBarracks = sender[args.index];
            selectActor = _selectOnBarracks.actor;
        }

        private void OnEnable()
        {
            valhalla.OnSelect += OnValhallaSelect;
            barracks.OnSelect += OnBarracksSelect;
        }

        private void OnDisable()
        {
            valhalla.OnSelect -= OnValhallaSelect;
            barracks.OnSelect -= OnBarracksSelect;
        }

    }
	
}
