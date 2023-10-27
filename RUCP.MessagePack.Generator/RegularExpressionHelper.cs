using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RUCP.MessagePack.Generator
{
    internal static class RegularExpressionHelper
    {
        private static Regex m_arrayPattern = new Regex(@"\w*\[]");
        private static Regex m_listPattern = new Regex(@"List<\w*>");

       

        internal static bool IsArray(string expression) 
        {
           return m_arrayPattern.IsMatch(expression);
        }

        internal static bool IsList(string fieldType)
        {
           return m_listPattern.IsMatch(fieldType);
        }
        internal static string GetBaseTypeFromList(string fieldType)
        {
            return m_listPattern.Replace(fieldType, "");
        }
    }
}
