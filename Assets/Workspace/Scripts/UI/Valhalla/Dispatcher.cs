// <copyright file="Dispatcher.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

using V4F.Character;
using V4F.Game;

namespace V4F.UI.Valhalla
{

    public class Dispatcher : MonoBehaviour
    {
        public delegate void HeroSelectedEventHandler(Actor hero);

        public event HeroSelectedEventHandler OnHeroSelected;

        public ValhallaList valhalla;
        public BarracksList barracks;
        public Button payButton;
        public GameObject GUI;

        private ValhallaItem _selectOnValhalla = null;
        private ValhallaItem _selectOnBarracks = null;
        private Actor _selectActor = null;
        private int _selectIndex = -1;

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
            _selectIndex = args.index;

            selectActor = _selectOnValhalla.actor;

            payButton.disable = false;
        }

        private void OnBarracksSelect(ListBox<ValhallaItem> sender, ListBoxEventArgs args)
        {
            if (_selectOnValhalla != null)
            {
                valhalla.ResetSelection();
                _selectOnValhalla = null;
            }

            _selectOnBarracks = sender[args.index];
            _selectIndex = args.index;

            selectActor = _selectOnBarracks.actor;

            payButton.disable = true;
        }

        private void OnPayClick(Button sender, ButtonEventArgs args)
        {
            if (_selectOnValhalla != null)
            {
                valhalla.RemoveAt(_selectIndex);
                _selectOnValhalla = null;

                selectActor.location = Location.Reserve;
                
                var index = barracks.AddItem(out _selectOnBarracks);
                _selectOnBarracks.actor = selectActor;
                barracks.SelectItem(index);
            }
        }

        private void OnEnable()
        {
            valhalla.OnSelect += OnValhallaSelect;
            barracks.OnSelect += OnBarracksSelect;
            payButton.OnClick += OnPayClick;
        }

        private void OnDisable()
        {
            valhalla.OnSelect -= OnValhallaSelect;
            barracks.OnSelect -= OnBarracksSelect;
            payButton.OnClick -= OnPayClick;            
        }

    }
	
}
