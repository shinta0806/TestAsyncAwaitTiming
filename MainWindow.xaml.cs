using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
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

namespace TestAsyncAwaitTiming
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

		private const Int32 DELAY_TIME = 1000;
		private const String URL = "https://www.instagram.com/";

		private HttpClient _httpClient = new();

		private void Write(String message, [CallerMemberName] String memberName = "")
		{
			Debug.WriteLine(DateTime.Now.ToString("ss.fff") + " / " + memberName + "() / " + message);
		}

		private void WriteHead(String message, [CallerMemberName] String memberName = "")
		{
			Write(message[..50].Replace("\n", String.Empty), memberName);
		}

		private async Task<String> SomeTaskAwaitAsync()
		{
			Write("Begin");

			// ここで時間がかかる
			String content = await _httpClient.GetStringAsync(URL);

			WriteHead(content);

			return content;
		}

		private async void ButtonSimple_Click(object sender, RoutedEventArgs e)
		{
			Write("Begin");

			// ここで時間がかかる
			String content = await _httpClient.GetStringAsync(URL);

			WriteHead(content);
		}

		private async void ButtonAwait_Click(object sender, RoutedEventArgs e)
		{
			Write("Begin");

			// SomeTaskAwaitAsync() の中で await しているが、ここでは時間はかからない
			Task<String> task = SomeTaskAwaitAsync();
			Write("After call");

			// ここで時間がかかる
			String content = await task;
			WriteHead(content);
		}
	}
}
