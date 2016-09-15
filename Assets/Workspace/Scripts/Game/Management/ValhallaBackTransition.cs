// <copyright file="ValhallaBackTransition.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

using V4F.FSM;

namespace V4F.Game.Management
{

    public class ValhallaBackTransition : StateTransition
    {
        public GameObject valhalla;
        public GameObject shading;
        public GameObject GUI;

        [Range(0f, 100f)]
        public float speedFading = 2f;

        public override IEnumerable ExitPrevState()
        {            
            shading.SetActive(false);

            var transform = valhalla.GetComponent<Transform>();
            var scale = 1f;

            while (scale > 0f)
            {
                transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
                scale -= Time.deltaTime * speedFading;
            }

            transform.localScale = Vector3.zero;
            GUI.SetActive(false);
            valhalla.SetActive(false);
        }

    }

}
