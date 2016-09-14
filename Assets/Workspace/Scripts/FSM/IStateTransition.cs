// <copyright file="IStateTransition.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

namespace V4F.FSM
{

    public interface IStateTransition
    {
        IEnumerable ExitPrevState();

        IEnumerable EnterNextState();
    }
	
}
