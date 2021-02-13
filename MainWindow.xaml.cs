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

namespace RegExPlorer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
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
				return;
			}
			if (regex.IsMatch(textToMatch))
			{
				blueCheck.Visibility = Visibility.Visible;
				redX.Visibility = Visibility.Collapsed;
			}
			else
			{
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
			string decimalNumber = "[+-]?((\\d+(\\.\\d*)?))";  // This is our best regex for matching a decimal number! 
																												 // This is our best match for a vector that looks like this: (1, 2, 3) -> (2, 2, 2)
																												 // ^\s*\(?\s*\d\s*,\s*\d\s*,\s*\d\s*\)?\s*->\s*\(?\s*\d\s*,\s*\d\s*,\s*\d\s*\)?\s*$

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
				Title = ex.Message;
				HideBlueCheckRedX();
				tbxRegExPattern.Background = new SolidColorBrush(Colors.Pink);
				iconInvalidRegEx.Visibility = Visibility.Visible;
			}
		}
		

		private void tbxText_TextChanged(object sender, TextChangedEventArgs e)
		{
			textToMatch = tbxText.Text;
			CheckForMatch();
		}
	}
}
