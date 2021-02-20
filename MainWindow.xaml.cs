using RegExPlorer;
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
		CodeGenerator godeGenerator = new CodeGenerator();
		public MainWindow()
		{
			InitializeComponent();
			tbxRegExPattern.Text = @"^((?<vectorName>\w+)\s*=\s*)?(\(?(?<X>[+-]?((\d+(\.\d+)?)))[, ]\s*(?<Y>[+-]?((\d+(\.\d+)?)))[, ]\s*(?<Z>[+-]?((\d+(\.\d+)?)))\s*\)?\s*->)?\s*\(?(?<deltaX>[+-]?((\d+(\.\d+)?)))[, ]\s*(?<deltaY>[+-]?((\d+(\.\d+)?)))[, ]\s*(?<deltaZ>[+-]?((\d+(\.\d+)?)))\s*\)?$";
			tbxText.Text = "a = (1, 2, 3) -> (2, 2, 2)";
			tbxClassName.Text = "MyClass";
		}

		Regex regex;
		string textToMatch;


		string GetTypeString(Match match, int i)
		{
			string value = match.Groups[i].Value;

			if (EvalHelper.GetPropertyType(value) == PropertyType.Double)
				return " (double)";

			return " (string)";
		}

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
						if (EvalHelper.IsGroupNameANumber(match, i))
							continue;
						groupName = match.Groups[i].Name;

						if (EvalHelper.GroupHasNoValue(match, i))
							continue;

						string type = "";  // GetTypeString(match, i);
						string value = match.Groups[i].Value;
						if (EvalHelper.GetPropertyType(value) == PropertyType.String)
							value = $"\"{value}\"";
						tbxGroupResults.Text += $"{groupName} = {value}{type}{Environment.NewLine}";
					}
				tbxCodeGen.Text = godeGenerator.GenerateCode(matches, tbxRegExPattern.Text, tbxClassName.Text, tbxText.Text);
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
			RegularExpressionHasChanged();
		}

		private void RegularExpressionHasChanged()
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

			string comma = "[, ]";
			string vectorStr = $"\\(?{Group(prefix + "X", numStr)}{comma}\\s*{Group(prefix + "Y", numStr)}{comma}\\s*{Group(prefix + "Z", numStr)}\\s*\\)?";
			return str.Replace(STR_Vector, vectorStr);
		}

		private static string Group(string name, string decimalNumber)
		{
			return $"(?<{name}>{decimalNumber})";
		}

		private const string assignment = @"((?<vectorName>\w+)\s*=\s*)?";

		private void VectorLong_Click(object sender, RoutedEventArgs e)
		{
			tbxRegExPattern.Text = $"^{assignment}({MakeVector(STR_Vector)}\\s*->)?\\s*{MakeVector(STR_Vector, "delta")}$";
		}

		private void tbxClassName_TextChanged(object sender, TextChangedEventArgs e)
		{
			CheckForMatch();
		}

		private void btnCopy_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(tbxCodeGen.Text);
			MessageBox.Show("Copied", "Good Luck!");
		}
	}
}
