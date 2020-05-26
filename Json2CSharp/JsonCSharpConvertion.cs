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
            for (int i = 0; i < inputLines.Length; i++)
            {

                var newLine = string.Empty;
                var n = inputLines[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.Trim().Length > 0).Select(X => X.Trim()).ToList();
                if (inputLines[i].Contains("{"))
                {
                    if (n.Count == 1)
                    {
                        newLine = $"{GetIntent(classNum)}{{";
                    }
                    else
                    {
                        var propertyName = n[0].ClearPropertyName();
                        var leftPart = $"{propertyName}";
                        var rightPart = $"new {propertyName.GetClassName(inputLines, i)}";
                        var classIntent = $"{Environment.NewLine}{GetIntent(classNum)}{{";

                        newLine = $"{GetIntent(classNum)}{leftPart} = {rightPart}{classIntent}";
                    }

                    classNum++;
                }
                else if (inputLines[i].Contains("["))
                {
                    var propertyName = n[0].ClearPropertyName();
                    var leftPart = $"{propertyName}";
                    var firstLineRightPart = $"new List<{propertyName.GetClassName(inputLines, i).ToSingle()}>";
                    var classIntent = $"{Environment.NewLine}{GetIntent(classNum)}{{";



                    if (inputLines[i].Contains("]"))
                    {
                        newLine = $"{GetIntent(classNum)}{leftPart} = {firstLineRightPart}()";
                    }
                    else
                    {
                        var firstLine = $"{GetIntent(classNum)}{leftPart} = {firstLineRightPart}{classIntent}";
                        classNum++;
                        var secondLineRightPart = $"new {propertyName.GetClassName(inputLines, i).ToSingle()}";
                        var secondLine = $"{GetIntent(classNum)}{secondLineRightPart}";

                        newLine = $"{firstLine}{Environment.NewLine}{secondLine}";
                    }
                }
                else if (inputLines[i].Contains("]"))
                {
                    classNum--;
                    newLine = $"{GetIntent(classNum)}{inputLines[i].Trim().Replace("]", "}")}";
                }
                else if (inputLines[i].Contains("}"))
                {
                    classNum--;
                    newLine = $"{GetIntent(classNum)}{inputLines[i].Trim()}";
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
