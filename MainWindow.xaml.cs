using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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

		private const String URL = "https://www.instagram.com/";

		private HttpClient _httpClient = new();

		private void Write(String message, [CallerMemberName] String memberName = "")
		{
			Debug.WriteLine(DateTime.Now.ToString("ss.fff") + " / T" + Environment.CurrentManagedThreadId + " / " + memberName + "() / " + message);
		}

		private void WriteHead(String message, [CallerMemberName] String memberName = "")
		{
			Write(message[..50].Replace("\n", String.Empty), memberName);
		}

		private async void ButtonSimple_Click(object sender, RoutedEventArgs e)
		{
			Write("Begin");

			// ここで時間がかかる
			String content = await _httpClient.GetStringAsync(URL);

			WriteHead(content);
		}

		private async Task<String> SomeTaskAwaitAsync()
		{
			Write("Begin");

			// ここで時間がかかる
			String content = await _httpClient.GetStringAsync(URL);

			WriteHead(content);

			return content;
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

		private async Task<String> SomeTaskAwaitAndSleepAsync()
		{
			Write("Begin");

			// ここで時間がかかる
			String content = await _httpClient.GetStringAsync(URL);

			WriteHead(content);

			// 時間のかかる処理がタスクになっていない場合のイメージ。ここでも時間がかかる
			Thread.Sleep(1000);
			Write("After Sleep");

			return content;
		}

		private async void ButtonAwaitSleep_Click(object sender, RoutedEventArgs e)
		{
			Write("Begin");

			// SomeTaskAwaitAsync() の中で await と Sleep しているが、ここでは時間はかからない
			Task<String> task = SomeTaskAwaitAndSleepAsync();
			Write("After call");

			// ここで時間がかかる
			String content = await task;
			WriteHead(content);
		}
	}
}
