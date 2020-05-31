using System;
using System.Linq;

namespace Json2CSharpLib
{
    public static class Extensions
    {
        public static string GetClassName(this string n, string[] inputLines, int currentLine)
        {
            var className = n;

            if (className.Contains("Audit"))
            {
                return "AuditData";
            }

            for (int i = currentLine + 2; i < inputLines.Length; i++)
            {
                if (inputLines[i].IndexOf("schemeData", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "Classification";
                }
                else if (inputLines[i].Contains('}') || inputLines[i].Contains('{'))
                {
                    break;
                }
            }

            return className;
        }

        public static string ClearPropertyName(this string n)
        {
            return n.Trim('"').FirstCharToUpper().Split('[').First();
        }

        public static string ToSingle(this string rightPart)
        {
            if (rightPart.EndsWith("List"))
            {
                return rightPart.Substring(0, rightPart.Length - 4);
            }
            else if (rightPart.EndsWith("es"))
            {
                return rightPart.Substring(0, rightPart.Length - 2);
            }
            else if (rightPart.EndsWith("s"))
            {
                return rightPart.Substring(0, rightPart.Length - 1);
            }
            else
            {
                return rightPart;
            }
        }


        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
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
            return rightPart.Replace("-", "").Replace("\"", "").Length == 32 && rightPart.Split('-').Length == 5;
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
