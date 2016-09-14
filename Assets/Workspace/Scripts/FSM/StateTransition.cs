// <copyright file="StateTransition.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

using UnityEngine;

namespace V4F.FSM
{

    public class StateTransition : MonoBehaviour, IStateTransition
    {
        public virtual IEnumerable EnterNextState()
        {
            yield return null;
        }

        public virtual IEnumerable ExitPrevState()
        {
            yield return null;
        }

    }
	
}
