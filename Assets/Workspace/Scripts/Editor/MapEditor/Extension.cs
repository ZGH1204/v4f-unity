// <copyright file="Extension.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

namespace V4F.MapEditor
{

    public static class Extension
    {
        public static bool Exists(this ITool self)
        {
            return (self != null);
        }

    }
	
}
