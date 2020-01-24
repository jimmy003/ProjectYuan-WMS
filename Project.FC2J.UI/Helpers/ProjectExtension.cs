using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.UI.Helpers
{
    public static class ProjectExtension
    {
        public static string ReplaceUnwantedCharacters(this string value)
        {
            return value.Replace("(","-")
                .Replace(")", "-")
                .Replace("[", "-")
                .Replace("]", "-")
                .Replace("/", "-")
                .Replace("\\", "-");
        }
        public static string MakeOneWord(this string value)
        {
            return value.Replace("(", "")
                .Replace(")", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace("/", "")
                .Replace(" ", "")
                .Replace("\\", "");
        }
    }
}
