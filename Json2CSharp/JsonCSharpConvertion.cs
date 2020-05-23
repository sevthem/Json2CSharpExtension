using System;
using System.Linq;
using System.Text;

namespace Json2CSharpLib
{
    public static class JsonCSharpConvertion
    {
        public static string Convert(string input)
        {
            var inputLines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var output = new StringBuilder();
            var classNum = 0;
            foreach (var line in inputLines)
            {

                var newLine = string.Empty;
                var n = line.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.Trim().Length > 0).Select(X => X.Trim()).ToList();
                if (line.Contains("{"))
                {
                    if (n.Count == 1)
                    {
                        newLine = "{";
                    }
                    else
                    {
                        var className = n[0].Trim('"').FirstCharToUpper();
                        var leftPart = $"{className}";
                        var rightPart = $"new {className}";
                        var classIntent = $"{Environment.NewLine}{GetIntent(classNum)}{{";

                        newLine = $"{GetIntent(classNum)}{leftPart} = {rightPart}{classIntent}";
                    }

                    classNum++;
                }
                else if (line.Contains("}"))
                {
                    classNum--;
                    newLine = $"{GetIntent(classNum)}{line.Trim()}";
                }
                else
                {
                    var leftPart = n[0].Trim('"').FirstCharToUpper();
                    var rightPart = n[1];
                    var comma = rightPart.GetComma();
                    rightPart = rightPart.ClearComma();
                    if (rightPart.IsDate())
                    {
                        rightPart = rightPart.GetDate();
                    }
                    else if (rightPart.IsDecimal())
                    {
                        rightPart = rightPart.GetDecimal();
                    }
                    else if (rightPart.IsGuid())
                    {
                        rightPart = rightPart.GetGuid();
                    }
                    newLine = $"{GetIntent(classNum)}{leftPart} = {rightPart}{comma}";
                }


                output.AppendLine(newLine);
            }

            return output.ToString();
        }

        private static string GetIntent(int classNum)
        {
            return new string('\t', classNum);
        }
    }
}
