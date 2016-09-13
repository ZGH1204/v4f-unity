// <copyright file="Extension.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections;

namespace V4F.FSM
{

    public static class Extension
    {
        public static bool Waiting(this IStateTransition self)
        {
            return (self == null);
        }

        public static bool Exists(this IState self)
        {
            return (self != null);
        }
    }
	
}
