namespace WeatherApp;

public partial class MapsPage : ContentPage
{
	public MapsPage()
	{
		InitializeComponent();
	}

	public async void OpenMap(object sender, EventArgs e)
	{
		string query = "shops";
		Uri uri = new($"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(query)}");
		await Launcher.OpenAsync(uri);
	}
}