using System;
using System.Linq;

namespace Json2CSharpLib
{
    public static class Extensions
    {
        public static string ClearPropertyName(this string n)
        {
            return n.Trim('"').FirstCharToUpper().Split('[').First();
        }
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return input[0].ToString().ToUpper() + input.Substring(1);
        }
        public static bool IsGuid(this string rightPart)
        {
            return rightPart.Replace("-", "").Replace("\"", "").Length == 32 && rightPart.Split('-').Length == 5;
        }
        public static string GetGuid(this string rightPart)
        {
            return $"Guid.Parse({rightPart})";
        }
        public static string GetDate(this string rightPart)
        {
            return $"DateTime.Parse({rightPart})";
        }
    }
}
