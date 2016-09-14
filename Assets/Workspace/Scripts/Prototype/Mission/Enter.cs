// <copyright file="Enter.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Prototype.Mission
{

    public class Enter : Section
    {        
        public GameObject doorButton;

        protected override void OnTriggerEnter(Collider other)
        {
            doorButton.SetActive(true);
        }

        protected override void OnTriggerExit(Collider other)
        {
            doorButton.SetActive(false);
        }

        private void OnDisable()
        {
            doorButton.SetActive(false);
        }
    }
	
}
