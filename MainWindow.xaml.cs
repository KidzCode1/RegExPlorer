using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeleteRegExDemoPrep
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		const string STR_Vector = "%vector%";
		public MainWindow()
		{
			InitializeComponent();
		}
		Regex regex;
		string textToMatch;
		void CheckForMatch()
		{
			if (textToMatch == null)
			{
				HideBlueCheckRedX();
				tbxGroupResults.Text = "";
				return;
			}
			if (regex.IsMatch(textToMatch))
			{
				blueCheck.Visibility = Visibility.Visible;
				redX.Visibility = Visibility.Collapsed;
				MatchCollection matches = regex.Matches(textToMatch);
				tbxGroupResults.Text = "";
				foreach (Match match in matches)
					for (int i = 1; i < match.Groups.Count; i++)
					{
						string groupName = string.Empty;
						if (int.TryParse(match.Groups[i].Name, out int result))
							continue;
						groupName = match.Groups[i].Name;

						if (string.IsNullOrEmpty(match.Groups[i].Value))
							continue;

						tbxGroupResults.Text += $"{groupName} = {match.Groups[i]}{Environment.NewLine}";
					}
			}
			else  // We don't have a match.
			{
				tbxGroupResults.Text = "";
				blueCheck.Visibility = Visibility.Collapsed;
				redX.Visibility = Visibility.Visible;
			}
		}

		void HideBlueCheckRedX()
		{
			blueCheck.Visibility = Visibility.Collapsed;
			redX.Visibility = Visibility.Collapsed;
		}

		private void tbxRegExPattern_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				regex = new Regex(tbxRegExPattern.Text);
				CheckForMatch();
				Title = "RegEx looks good!";
				iconInvalidRegEx.Visibility = Visibility.Collapsed;
				tbxRegExPattern.Background = new SolidColorBrush(Colors.White);
			}
			catch (Exception ex)
			{
				tbxGroupResults.Text = "";
				Title = ex.Message;
				HideBlueCheckRedX();
				tbxRegExPattern.Background = new SolidColorBrush(Colors.Pink);
				iconInvalidRegEx.Visibility = Visibility.Visible;
			}
		}
		/* We found this expression was great for matching our proposed vector syntax:
		 
				^\(?\s*\d\s*,\s*\d\s*,\s*\d\s*\)?$
		 
		 */
		private void tbxText_TextChanged(object sender, TextChangedEventArgs e)
		{
			textToMatch = tbxText.Text;
			CheckForMatch();
		}

		string MakeVector(string str, string prefix = "")
		{
			string numStr = @"[+-]?((\d+(\.\d+)?))";  // This is our best regex for matching a decimal number! 
																								// This is our best match for a vector that looks like this: (1, 2, 3) -> (2, 2, 2)
																								// ^\s*\(?\s*\d\s*,\s*\d\s*,\s*\d\s*\)?\s*->\s*\(?\s*\d\s*,\s*\d\s*,\s*\d\s*\)?\s*$
			string vectorStr = $"\\(?{Group(prefix + "X", numStr)},\\s*{Group(prefix + "Y", numStr)},\\s*{Group(prefix + "Z", numStr)}\\s*\\)?";
			return str.Replace(STR_Vector, vectorStr);
		}

		private static string Group(string name, string decimalNumber)
		{
			return $"(?<{name}>{decimalNumber})";
		}

		private const string assignment = @"((?<var>\w+)\s*=\s*)?";

		private void VectorLong_Click(object sender, RoutedEventArgs e)
		{
			tbxRegExPattern.Text = $"^{assignment}({MakeVector(STR_Vector)}\\s*->)?\\s*{MakeVector(STR_Vector, "delta")}$";
		}
	}
}
