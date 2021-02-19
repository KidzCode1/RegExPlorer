using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeleteRegExDemoPrep
{
	public class CodeGenerator
	{
		private const string createBody = @"    RegExResult regExResult = new RegExResult();

    Regex regex = new Regex(pattern);
    MatchCollection matches = regex.Matches(str);

";

		private const string createMethodStart = @"
  public static RegExResult Create(string str)
  {
";

		private const string createMethodEnd = @"  }
";

		private const string classStart = @"public class RegExResult
{
";

		private const string classEnd = @"}

";

		private const string HelperMethod = @"public static class RegexHelper
{
  public static T GetValue<T>(MatchCollection matches, string groupName)
  {
    foreach (Match match in matches)
      for (int i = 1; i < match.Groups.Count; i++)
      {
        if (match.Groups[i].Name == groupName)
        {
          string value = match.Groups[i].Value;

          if (typeof(T).Name == typeof(double).Name)
            if (double.TryParse(value, out double result))
              return (T)(object)result;

          return (T)(object)value;
        }
      }
    return default(T);
  }
}";

		public CodeGenerator()
		{

		}

		public string GenerateCode(MatchCollection matches, string pattern, string className, string sampleText)
		{
			string result = "";
			string instanceName = GetInstanceName(ref className);

			result += classStart.Replace("RegExResult", className);
			result = AddProperties(matches, result);
			result = AddCreateMethod(matches, pattern, className, result, instanceName);
			result += classEnd;
			result += "// Sample usage: " + Environment.NewLine;
			result += $"// {className} {instanceName} = {className}.Create(\"{sampleText}\");" + Environment.NewLine + Environment.NewLine;
			result += HelperMethod;
			return result;
		}

		private static string AddCreateMethod(MatchCollection matches, string pattern, string className, string result, string instanceName)
		{
			result += createMethodStart.Replace("RegExResult", className); ;
			result += $"    const string pattern = @\"{pattern}\";{Environment.NewLine}";
			result += createBody.Replace("RegExResult", className).Replace("regExResult", instanceName);
			result = AddInitialization(matches, result);
			result += createMethodEnd;
			return result;
		}

		private static string GetInstanceName(ref string className)
		{
			if (string.IsNullOrWhiteSpace(className) || className.Length <= 1)
				className = "MyClass";
			className = className.Replace(' ', '_');
			string instanceName = char.ToLower(className[0]) + className.Substring(1);
			if (!char.IsUpper(className[0]))
				className = char.ToUpper(className[0]) + className.Substring(1);
			return instanceName;
		}

		private static string AddProperties(MatchCollection matches, string result)
		{
			foreach (Match match in matches)
				for (int i = 1; i < match.Groups.Count; i++)
				{
					if (EvalHelper.IsGroupNameANumber(match, i))
						continue;

					if (EvalHelper.GroupHasNoValue(match, i))
						continue;

					string groupName = match.Groups[i].Name;

					PropertyType type = EvalHelper.GetPropertyType(match.Groups[i].Value);
					string typeStr = EvalHelper.GetPropertyTypeStr(type);
					result += $"  public {typeStr} {groupName} " + "{ get; set; }" + Environment.NewLine;
				}

			return result;
		}

		private static string AddInitialization(MatchCollection matches, string result)
		{
			foreach (Match match in matches)
				for (int i = 1; i < match.Groups.Count; i++)
				{
					if (EvalHelper.IsGroupNameANumber(match, i))
						continue;

					if (EvalHelper.GroupHasNoValue(match, i))
						continue;

					string groupName = match.Groups[i].Name;

					PropertyType type = EvalHelper.GetPropertyType(match.Groups[i].Value);
					string typeStr = EvalHelper.GetPropertyTypeStr(type);
					result += $"    rexExResult.{groupName} = RegexHelper.GetValue<{typeStr}>(matches, \"{groupName}\");" + Environment.NewLine;
				}

			return result;
		}
	}
}
