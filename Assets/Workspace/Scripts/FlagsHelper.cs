// <copyright file="FlagsHelper.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

namespace V4F
{

    public class FlagsHelper
    {
        #region Методы
        public static bool IsSet<T>(T flags, T flag) where T : struct
        {
            var flagsValue = (int)(object)flags;
            var flagValue = (int)(object)flag;

            return (flagsValue & flagValue) != 0;
        }

        public static void Set<T>(ref T flags, T flag) where T : struct
        {
            var flagsValue = (int)(object)flags;
            var flagValue = (int)(object)flag;

            flags = (T)(object)(flagsValue | flagValue);
        }

        public static void Unset<T>(ref T flags, T flag) where T : struct
        {
            var flagsValue = (int)(object)flags;
            var flagValue = (int)(object)flag;

            flags = (T)(object)(flagsValue & (~flagValue));
        }
        #endregion
    }

}