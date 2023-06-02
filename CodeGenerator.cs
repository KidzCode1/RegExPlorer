using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeleteRegExDemoPrep
{
    public class CodeGenerator
	{
        private const string getMatchesStart = @"
  /// <summary>
  /// Collects regular expression matches for the specified input text.
  /// </summary>
  /// <param name=""input"">The input text to get a match for. For example, #SampleInput#.</param>
  public static MatchCollection GetMatches(string input)
  {
";
        private const string getMatchesBody = @"
    Regex regex = new Regex(pattern);
    return regex.Matches(input);
  }
";

		private const string createMethodStart = @"
  /// <summary>
  /// Creates a new RegExResult based on the specified input text.
  /// </summary>
  /// <param name=""input"">The input text to get a match for. For example, #SampleInput#.</param>
  /// <returns>Returns the new RegExResult, or null if no matches were found for the specified input.</returns>
  public static RegExResult? Create(string input)
  {
    MatchCollection matches = GetMatches(input);
    if (matches.Count == 0)
      return null;

    RegExResult regExResult = new RegExResult();
";

		private const string createMethodEnd = @"
  }
";

		private const string classStart = @"public class RegExResult
{
";

		private const string classEnd = @"}

";

        private const string HelperMethod = @"public static class RegexHelper
{
  public static T GetValue<T>(MatchCollection matches, string groupName, T defaultValueIfNotFound = default(T))
  {
    foreach (Match match in matches)
    {
      GroupCollection groups = match.Groups;
      Group group = groups[groupName];
      if (group == null)
        continue;

      string value = group.Value;

      if (string.IsNullOrEmpty(value))
        return defaultValueIfNotFound;

      if (typeof(T).Name == typeof(double).Name)
        if (double.TryParse(value, out double result))
          return (T)(object)result;

      return (T)(object)value;
    }

    return defaultValueIfNotFound;
  }
}";

		public CodeGenerator()
		{

		}

		public string GenerateCode(MatchCollection matches, string pattern, string className, string sampleText, bool includeRegExHelper)
		{
			string result = "";
			string instanceName = GetInstanceName(ref className);

			result += classStart.Replace("RegExResult", className);
			result = AddProperties(matches, result);
			result = AddGetMatchesAndCreateMethods(matches, pattern, className, result, instanceName, sampleText);
			result += classEnd;
			result += "// Sample usage: " + Environment.NewLine;
			result += $"// {className} {instanceName} = {className}.Create(\"{sampleText.Replace("\"", "\\\"")}\");" + Environment.NewLine + Environment.NewLine;
			if (includeRegExHelper)
				result += HelperMethod;
			return result;
		}

        static string FormatForXmlDocComment(string sampleText)
        {
            string converted = sampleText.Replace("<", "&lt;").Replace(">", "&gt;");
            if (converted.IndexOf('"') < 0)
                return $"\"{converted}\"";
            if (converted.IndexOf("'") < 0)
                return $"'{converted}'";
            return converted;
        }
        private static string AddGetMatchesAndCreateMethods(MatchCollection matches, string pattern, string className, string result, string instanceName, string sampleText)
		{
            result += getMatchesStart.Replace("#SampleInput#", FormatForXmlDocComment(sampleText));
            result += $"    const string pattern = @\"{pattern.Replace("\"", "\"\"")}\";{Environment.NewLine}";
            result += getMatchesBody;
            result += createMethodStart.Replace("RegExResult", className).Replace("regExResult", instanceName).Replace("#SampleInput#", FormatForXmlDocComment(sampleText));
            result = AddInitialization(matches, result, instanceName);
			result += $"    return {instanceName};";
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
					result += $"  public {typeStr}? {groupName} " + "{ get; set; }" + Environment.NewLine;
				}

			return result;
		}

		private static string AddInitialization(MatchCollection matches, string result, string instanceName)
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
					result += $"    {instanceName}.{groupName} = RegexHelper.GetValue<{typeStr}>(matches, \"{groupName}\");" + Environment.NewLine;
				}

			return result;
		}
	}
}
