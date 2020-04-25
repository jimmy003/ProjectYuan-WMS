using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Project.FC2J.UI.Helpers
{
    internal static class HelperExtensions
    {
        public static bool AndGridWillBeBack(this int value)
        {
            Thread.Sleep(value * 1000);
            return true;
        }

        public static T CloneObject<T>(this object source)
        {
            T result = Activator.CreateInstance<T>();
            //// **** made things  
            return result;
        }

    }
}
