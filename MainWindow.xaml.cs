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
		private void tbxRegExPattern_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				regex = new Regex(tbxRegExPattern.Text);
				CheckForMatch();
				Title ="RegEx Looks Good!";
			}
			catch (Exception ex)
			{
				Title = ex.Message;
			}
		}

		private void tbxText_TextChanged(object sender, TextChangedEventArgs e)
		{
			textToMatch = tbxText.Text;
			CheckForMatch();
		}
	}
}
