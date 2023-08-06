using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameSaveManagement.Services
{
    public static class Extensions
    {
        public static string RemoveQuote(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }
            return str.Replace("\"", "");
        }
    }
}
