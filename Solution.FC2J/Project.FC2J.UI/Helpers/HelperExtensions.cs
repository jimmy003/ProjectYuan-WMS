using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project.FC2J.UI.Helpers
{
    internal static class HelperExtensions
    {
        public static bool AndGridWillBeBack(this int value)
        {
            Thread.Sleep(value * 1000);
            return true;
        }

    }
}
