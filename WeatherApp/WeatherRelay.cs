using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace WeatherApp;

internal static class WeatherRelay
{
	static readonly string serverAddress = "https://weatherapp-8jw4.onrender.com";
	static readonly HttpClient httpClient = new() { BaseAddress = new(serverAddress) };
	static readonly JsonSerializerOptions pretty = new() { WriteIndented = true };

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
}

internal class JsonResponse(HttpStatusCode StatusCode, JsonDocument? JsonDocument) : IDisposable
{
	static readonly JsonSerializerOptions pretty = new() { WriteIndented = true };

	public void Dispose() => JsonDocument?.Dispose();

	public override string ToString() =>
		StatusCode.HasFlag(HttpStatusCode.OK) ? JsonSerializer.Serialize(JsonDocument!.RootElement, pretty) : StatusCode.ToString();

}
