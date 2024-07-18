#pragma warning disable CS1998

using SocketIOClient;

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
			Console.WriteLine("Type Quit to exit...");
			while (Console.ReadLine()?.ToLower().Trim() != "quit") ;
			await client.DisconnectAsync();
		}
	}
}

/*
curl POST https://weatherapp-8jw4.onrender.com/publish --data '{}' -H "Content-type: application/json" -v
*/