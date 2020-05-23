using System;
using System.Linq;

namespace Json2CSharpLib
{
    public static class Extensions
    {
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        public static string GetDecimal(this string rightPart)
        {
            return $"{rightPart}m";
        }

        public static bool IsDecimal(this string rightPart)
        {
            return !rightPart.Contains('"') && rightPart.Contains('.');
        }

        public static bool IsGuid(this string rightPart)
        {
            return rightPart.Replace("-","").Replace("\"","").Length == 32  && rightPart.Split('-').Length == 5;
        }
        public static string GetGuid(this string rightPart)
        {
            return $"Guid.Parse({rightPart})";
        }

        public static string ClearComma(this string rightPart)
        {
            return rightPart.Trim(',');
        }

        public static string GetComma(this string rightPart)
        {
            return rightPart.EndsWith(",") ? "," : string.Empty;
        }

        public static string GetDate(this string rightPart)
        {
            return $"DateTime.Parse({rightPart})";
        }

        public static bool IsDate(this string rightPart)
        {
            return rightPart.Split('-').Length == 3;
        }
    }
}
