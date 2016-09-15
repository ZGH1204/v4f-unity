// <copyright file="PlayHandler.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using V4F.Prototype.Mission;
using V4F.Prototype.Map;

namespace V4F.UI.Mission
{

    public class PlayHandler : MonoBehaviour
    {
        public Section enter;
        public Section exit;
        public Section room;

        public GameObject enterButton;
        public GameObject exitButton;
        public GameObject mapButton;

        public void Entry(NodeType type)
        {
            gameObject.SetActive(true);
        }

        public void Exit()
        {
            enterButton.SetActive(false);
            exitButton.SetActive(false);
            mapButton.SetActive(false);
            gameObject.SetActive(false);
        }

        private void SectionEnterHandler(Section sender, SectionEventArgs args)
        {
            if (args.type == SectionType.Enter)
            {
                enterButton.SetActive(true);
            }
            else if (args.type == SectionType.Exit)
            {
                exitButton.SetActive(true);
            }
            else if (args.type == SectionType.Room)
            {
                mapButton.SetActive(true);
            }
        }

        private void SectionExitHandler(Section sender, SectionEventArgs args)
        {
            if (args.type == SectionType.Enter)
            {
                enterButton.SetActive(false);
            }
            else if (args.type == SectionType.Exit)
            {
                exitButton.SetActive(false);
            }
            else if (args.type == SectionType.Room)
            {
                mapButton.SetActive(false);
            }
        }

        private void OnEnable()
        {
            enter.OnEnter += SectionEnterHandler;
            enter.OnExit += SectionExitHandler;
            exit.OnEnter += SectionEnterHandler;
            exit.OnExit += SectionExitHandler;
            room.OnEnter += SectionEnterHandler;
            room.OnExit += SectionExitHandler;
        }

        private void OnDisable()
        {
            enter.OnEnter -= SectionEnterHandler;
            enter.OnExit -= SectionExitHandler;
            exit.OnEnter -= SectionEnterHandler;
            exit.OnExit -= SectionExitHandler;
            room.OnEnter -= SectionEnterHandler;
            room.OnExit -= SectionExitHandler;
        }
    }
	
}
