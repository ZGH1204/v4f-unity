// <copyright file="ValhallaTransition.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

using V4F.FSM;

namespace V4F.Game.Management
{

    public class ValhallaTransition : StateTransition
    {
        public GameObject valhalla;
        public GameObject shading;

        [Range(0f, 100f)]
        public float speedFading = 2f;

        public override IEnumerable EnterNextState()
        {
            valhalla.SetActive(true);
            shading.SetActive(false);

            var transform = valhalla.GetComponent<Transform>();
            var scale = 0f;

            while (scale < 1f)
            {
                transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
                scale += Time.deltaTime * speedFading;
            }

            transform.localScale = Vector3.one;
            shading.SetActive(true);
        }

    }
	
}
