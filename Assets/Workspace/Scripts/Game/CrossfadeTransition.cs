// <copyright file="CrossfadeTransition.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using V4F.FSM;

namespace V4F.Game
{

    public class CrossfadeTransition : StateTransition
    {
        public Image placeholder = null;

        [Range(0f, 10f)]
        public float multiplier = 1f;

        public override IEnumerable EnterNextState()
        {
            foreach (var e in FadeIn())
            {
                yield return e;
            }

            gameObject.SetActive(false);
        }

        public override IEnumerable ExitPrevState()
        {
            gameObject.SetActive(true);

            foreach (var e in FadeOut())
            {
                yield return e;
            }
        }

        private IEnumerable FadeIn()
        {
            var color = new Color(0f, 0f, 0f, 1f);
            while (color.a > 0f)
            {
                placeholder.color = color;
                yield return null;
                color.a -= Time.deltaTime * multiplier;
            }

            placeholder.color = new Color(0f, 0f, 0f, 0f);
        }

        private IEnumerable FadeOut()
        {
            var color = new Color(0f, 0f, 0f, 0f);
            while (color.a < 1f)
            {
                placeholder.color = color;
                yield return null;
                color.a += Time.deltaTime * multiplier;
            }

            placeholder.color = new Color(0f, 0f, 0f, 1f);
        }
    }

}
