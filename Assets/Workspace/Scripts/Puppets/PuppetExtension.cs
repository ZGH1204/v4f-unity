// <copyright file="PuppetExtension.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using UnityEngine;

namespace V4F.Puppets
{

    public static class PuppetExtension
    {
        #region Methods
        public static int GetIndex(this Stats self)
        {
            return (int)self;
        }

        public static int GetIndex(this Resists self)
        {
            return (int)self;
        }
        #endregion
    }

}
