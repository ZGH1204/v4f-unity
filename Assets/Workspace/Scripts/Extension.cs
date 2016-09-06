// <copyright file="Extension.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

namespace V4F
{
    
    public static class Extension
    {
        public static bool DefaultInitializer<T>(this T[] array, int count) where T : class, new()
        {
            if (array == null)
            {
                array = new T[count];
                for (var i = 0; i < count; ++i)
                {
                    array[i] = new T();
                }
                return true;
            }
            return false;
        }

        public static bool DefaultInitializer<T>(this T[] array, int count, T defaultValue) where T : struct
        {
            if (array == null)
            {
                array = new T[count];
                for (var i = 0; i < count; ++i)
                {
                    array[i] = defaultValue;
                }
                return true;
            }
            return false;
        }
    }
	
}
