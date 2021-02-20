using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegExPlorer
{
	public static class RegexHelper
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
	}
	public class RegExResult
	{
		public double deltaX { get; set; }  // +
		public double deltaY { get; set; }  // +
		public double deltaZ { get; set; }  // +

		/// <summary>
		/// Creates a new RegExResult based on the specified input text.
		/// </summary>
		/// <param name="input">The input text to get a match for. For example, "#SampleInput#".</param>
		/// <returns>Returns the new RegExResult, or null if a no matches were found for the specified input.</returns>
		public static RegExResult Create(string input)
		{
			const string pattern = @"(?<deltaX>\d+), (?<deltaY>\d+), (?<deltaZ>\d+)";

			Regex regex = new Regex(pattern);
			MatchCollection matches = regex.Matches(input);
			if (matches.Count == 0)
				return null;

			RegExResult regExResult = new RegExResult();
			regExResult.deltaX = RegexHelper.GetValue<double>(matches, "deltaX");
			regExResult.deltaY = RegexHelper.GetValue<double>(matches, "deltaY");
			regExResult.deltaZ = RegexHelper.GetValue<double>(matches, "deltaZ");

			return regExResult;
		}
	}


}
