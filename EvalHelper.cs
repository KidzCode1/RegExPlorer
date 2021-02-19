using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeleteRegExDemoPrep
{
	public static class EvalHelper
	{
		public static PropertyType GetPropertyType(string value)
		{
			if (double.TryParse(value, out double _))
				return PropertyType.Double;
			return PropertyType.String;
		}

		public static bool GroupHasNoValue(Match match, int i)
		{
			return string.IsNullOrEmpty(match.Groups[i].Value);
		}

		public static bool IsGroupNameANumber(Match match, int i)
		{
			return int.TryParse(match.Groups[i].Name, out int result);
		}

		internal static string GetPropertyTypeStr(PropertyType type)
		{
			if (type == PropertyType.Double)
				return "double";
			return "string";
		}
	}
}
