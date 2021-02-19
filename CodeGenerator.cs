using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeleteRegExDemoPrep
{
	public class CodeGenerator
	{
		/* public static RegExResult Create(string str)
		{
			const string pattern = @"(?<deltaX>\d+), (?<deltaY>\d+), (?<deltaZ>\d+)";  // +
			RegExResult regExResult = new RegExResult();

			Regex regex = new Regex(pattern);
			MatchCollection matches = regex.Matches(str);

			regExResult.deltaX = RegexHelper.GetValue<double>(matches, "deltaX");  // +
			regExResult.deltaY = RegexHelper.GetValue<double>(matches, "deltaY");  // +
			regExResult.deltaZ = RegexHelper.GetValue<double>(matches, "deltaZ");  // +

			return regExResult;
		} */

		private const string createBody = @"		RegExResult regExResult = new RegExResult();

		Regex regex = new Regex(pattern);
		MatchCollection matches = regex.Matches(str);

";

		private const string createMethodStart = @"
	public static RegExResult Create(string str)
	{
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

		public string GenerateCode(MatchCollection matches, string pattern)
		{
			string result = "";
			result += classStart;
			result = AddProperties(matches, result);
			result += createMethodStart;
			result += $"\t\tconst string pattern = @\"{pattern}\";{Environment.NewLine}";
			result += createBody;
			result = AddInitialization(matches, result);
			result += createMethodEnd;
			result += classEnd;
			result += HelperMethod;
			return result;
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
					result += $"\tpublic {typeStr} {groupName} " + "{ get; set; }" + Environment.NewLine;
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
					result += $"\t\trexExResult.{groupName} = RegexHelper.GetValue<{typeStr}>(matches, \"{groupName}\");" + Environment.NewLine;
				}

			return result;
		}
	}
}
