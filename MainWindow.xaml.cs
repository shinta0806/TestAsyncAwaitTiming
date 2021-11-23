// ============================================================================
// 
// async/await の実行タイミングを追うためのテストプログラム
// 
// ============================================================================

// ----------------------------------------------------------------------------
// 挙動は以下のようになるようだ：
// (1) async な関数を呼びだした場合、async な関数の先頭から実行される
// (2) async な関数内のコードが await に到達した時点で、残りのコードをタスクとして返す
// (3) タスクが返ると、呼びだし元の関数は、後続のコードを直ちに実行継続する
// (4) 呼びだし元の関数で await に到達すると、async な関数のタスクが終了するのを待機する
//
// タスク取得にかかる時間：
// [a] async な関数のタスクを取得するだけ（await しない）なら、通常はほとんど時間がかからない
// [b] async な関数の先頭（await に到達する前）にヘビーなコードがある場合は、タスクを取得するだけでも時間がかかる
// ----------------------------------------------------------------------------

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
		// ====================================================================
		// コンストラクター
		// ====================================================================

		/// <summary>
		/// コンストラクター
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
		}

		// ====================================================================
		// private メンバー定数
		// ====================================================================

		// アクセス先
		private const String URL = "https://www.instagram.com/";

		// ====================================================================
		// private メンバー変数
		// ====================================================================

		// HTTP クライアント
		private HttpClient _httpClient = new();

		// ====================================================================
		// private メンバー関数
		// ====================================================================

		#region シンプルに await する場合
		/// <summary>
		/// シンプルに await する場合
		/// </summary>
		private async void ButtonSimple_Click(object sender, RoutedEventArgs e)
		{
			Write("Begin");

			// await で時間がかかる
			Write("awaiting...");
			String content = await _httpClient.GetStringAsync(URL);

			WriteHead(content);
		}
		#endregion

		#region 「内部で await している関数」を呼びだす場合
		/// <summary>
		/// 「内部で await している関数」を呼びだし、タスクを取得
		/// </summary>
		private async void ButtonAwait_Click(object sender, RoutedEventArgs e)
		{
			Write("Begin");

			// SomeTaskAwaitAsync() の中で await しているが、タスクを取得するだけなら時間はかからない
			Task<String> task = SomeTaskAwaitAsync();
			Write("After call");

			// await で時間がかかる
			Write("awaiting...");
			String content = await task;
			WriteHead(content);
		}

		/// <summary>
		/// 内部で await している関数
		/// </summary>
		private async Task<String> SomeTaskAwaitAsync()
		{
			Write("Begin");

			// await で時間がかかる
			Write("awaiting...");
			String content = await _httpClient.GetStringAsync(URL);

			WriteHead(content);

			return content;
		}
		#endregion

		#region 「内部で await と Sleep している関数」を呼びだす場合
		/// <summary>
		/// 「内部で await と Sleep している関数」を呼びだし、タスクを取得
		/// </summary>
		private async void ButtonAwaitSleep_Click(object sender, RoutedEventArgs e)
		{
			Write("Begin");

			// SomeTaskAwaitAndSleepAsync() の中で await と Sleep しているが、タスクを取得するだけなら時間はかからない
			Task<String> task = SomeTaskAwaitAndSleepAsync();
			Write("After call");

			// await で時間がかかる
			Write("awaiting...");
			String content = await task;
			WriteHead(content);
		}

		/// <summary>
		/// 内部で await と Sleep している関数
		/// </summary>
		private async Task<String> SomeTaskAwaitAndSleepAsync()
		{
			Write("Begin");

			// await で時間がかかる
			Write("awaiting...");
			String content = await _httpClient.GetStringAsync(URL);

			WriteHead(content);

			// 時間のかかる処理がタスクになっていない場合のイメージ。ここでも時間がかかる
			Thread.Sleep(1000);
			Write("After Sleep");

			return content;
		}
		#endregion

		#region 「内部で Sleep と await している関数」を呼びだす場合
		/// <summary>
		/// 「内部で Sleep と await している関数」を呼びだし、タスクを取得
		/// </summary>
		private async void ButtonSleepAwait_Click(object sender, RoutedEventArgs e)
		{
			Write("Begin");

			// SomeTaskSleepAndAwaitAsync() の中で Sleep と await しており、タスクを取得するだけでも最初の Sleep の部分だけ時間がかかる
			Task<String> task = SomeTaskSleepAndAwaitAsync();
			Write("After call");

			// await で SomeTaskSleepAndAwaitAsync() 後半の await の部分の時間がかかる
			Write("awaiting...");
			String content = await task;
			WriteHead(content);
		}

		/// <summary>
		/// 内部で Sleep と await している関数
		/// </summary>
		/// <returns></returns>
		private async Task<String> SomeTaskSleepAndAwaitAsync()
		{
			Write("Begin");

			// 時間のかかる処理がタスクになっていない場合のイメージ。ここで時間がかかる
			Thread.Sleep(1000);
			Write("After Sleep");

			// await で時間がかかる
			Write("awaiting...");
			String content = await _httpClient.GetStringAsync(URL);

			WriteHead(content);

			return content;
		}
		#endregion

		#region ユーティリティー
		/// <summary>
		/// メッセージ表示
		/// </summary>
		private void Write(String message, [CallerMemberName] String memberName = "")
		{
			Debug.WriteLine(DateTime.Now.ToString("ss.fff") + " / T" + Environment.CurrentManagedThreadId + " / " + memberName + "() / " + message);
		}

		/// <summary>
		/// メッセージの先頭部分のみ表示
		/// </summary>
		/// <param name="message"></param>
		/// <param name="memberName"></param>
		private void WriteHead(String message, [CallerMemberName] String memberName = "")
		{
			Write(message[..50].Replace("\n", String.Empty), memberName);
		}
		#endregion
	}
}
