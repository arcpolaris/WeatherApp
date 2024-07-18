#pragma warning disable CS1998

using SocketIOClient;
using System.Text;

namespace LocalUtilities
{
	internal static class SocketDebugger
	{
		static readonly string url = "https://weatherapp-8jw4.onrender.com";
		internal static async Task TestSocket()
		{
			var client = new SocketIOClient.SocketIO(url);
			client.OnConnected += async (sender, e) =>
			{
				Console.WriteLine("Connected");
			};
			client.On("alert event", async response =>
			{
				Console.WriteLine("Holy crap theres an alert: " + response.ToString());
			});

			await client.ConnectAsync();
			await Request();
			Console.WriteLine("Type Quit to exit...");
			while (Console.ReadLine()?.ToLower().Trim() != "quit") ;
			await client.DisconnectAsync();
		}

		static async Task Request()
		{
			string jsonPayload = "{\"text\":\"hello world\"}";

			using var client = new HttpClient();
			var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

			try
			{
				var response = await client.PostAsync($"{url}/publish", content);
				string responseContent = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"Response Status Code: {response.StatusCode}");
				Console.WriteLine($"Response Content: {responseContent}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
		}
	}
}