using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using IOSocket = SocketIOClient.SocketIO;
using Plugin.LocalNotification;
#if DEBUG
using System.Diagnostics;
#endif
namespace WeatherApp;

internal static class WeatherRelay
{
	public static readonly string serverAddress = "https://weatherapp-8jw4.onrender.com";
	static readonly HttpClient httpClient = new() { BaseAddress = new(serverAddress) };

	public static async Task<JsonResponse> Request(string endpoint, object? payload = null)
	{
		payload ??= new { };
		using StringContent content = new(
			JsonSerializer.Serialize(payload),
			Encoding.UTF8,
			"application/json");

		using HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);


		//Debug.WriteLine(response.StatusCode.ToString());
		if (response.StatusCode.HasFlag(HttpStatusCode.OK))
		{
			var jdoc = await response.Content.ReadFromJsonAsync<JsonDocument>();
			return new JsonResponse(response.StatusCode, jdoc);
		}
		return new JsonResponse(response.StatusCode, null);
	}

	public static async Task<GenericResponse<T>> Request<T>(string endpoint, object? payload = null)
	{
		var response = await Request(endpoint, payload);
		return response.JsonDocument == null
			? new(response.StatusCode, default)
			: new(response.StatusCode, response.JsonDocument!.Deserialize<T>());
	}
}

internal class JsonResponse(HttpStatusCode StatusCode, JsonDocument? JsonDocument) : IDisposable
{
	static readonly JsonSerializerOptions pretty = new() { WriteIndented = true };

	public HttpStatusCode StatusCode { get; } = StatusCode;
	public JsonDocument? JsonDocument { get; } = JsonDocument;

	public void Dispose() => JsonDocument?.Dispose();

	public override string ToString() =>
		StatusCode.HasFlag(HttpStatusCode.OK) ? JsonSerializer.Serialize(JsonDocument!.RootElement, pretty) : StatusCode.ToString();

}

internal record GenericResponse<T>(HttpStatusCode StatusCode, T? Value);

public sealed class WeatherRelaySocketService
{
	bool built;
	IOSocket? _socket = null;
	public IOSocket? Socket => _socket;

	public static bool TryBuild(WeatherRelaySocketService service)
	{
		if (service.built) return false;
		service.built = true;
		service._socket = new(WeatherRelay.serverAddress);
		return true;
	}

	public event EventHandler<NotificationRequest>? Notified;
	public void Notify(NotificationRequest request) => Notified?.Invoke(this, request);
}

public partial class App
{
#pragma warning disable CA2211
	public static WeatherRelaySocketService? relayService;
	public static Predicate<Dictionary<string, string>> relayPredicate = (_) => true;
#pragma warning restore CA2211
	private readonly WeatherRelaySocketService relaySocket;

	public App(WeatherRelaySocketService relaySocket) : this()
	{
		this.relaySocket = relaySocket;
		relayService = relaySocket;
	}

	protected override async void OnStart()
	{
		base.OnStart();
		WeatherRelaySocketService.TryBuild(relaySocket);
		relaySocket.Socket!.OnConnected += (sender, e) =>
		{
#if DEBUG
			Debug.WriteLine("Connected");
#endif
		};
		relaySocket.Socket!.On("alert event", async response =>
		{
#if DEBUG
			Debug.WriteLine(response.ToString());
#endif
			var root = response.GetValue<Dictionary<string, string>>();
			if (!relayPredicate(root)) return;

			string title, subtitle, description;
			{ if (root.TryGetValue("title", out var value)) title = value; else return; }
			{ if (root.TryGetValue("subtitle", out var value)) subtitle = value; else return; }
			{ if (root.TryGetValue("description", out var value)) description = value; else return; }
#if DEBUG
			Debug.WriteLine("Guards passed");
#endif
			NotificationRequest request = new()
			{
				NotificationId = response.GetHashCode(),
				Title = title,
				Subtitle = subtitle,
				Description = description,
				Schedule = new()
				{
					NotifyTime = DateTime.Now.AddSeconds(3),
				}
			};

			relaySocket.Notify(request);

			if (!await LocalNotificationCenter.Current.AreNotificationsEnabled())
				await LocalNotificationCenter.Current.RequestNotificationPermission();
#if DEBUG
			//await LocalNotificationCenter.Current.RequestNotificationPermission();
#endif
			await LocalNotificationCenter.Current.Show(request);
		});

		await relaySocket.Socket.ConnectAsync();
	}
}