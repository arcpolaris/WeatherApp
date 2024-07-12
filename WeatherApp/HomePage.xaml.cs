namespace WeatherApp;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
	}

	async void LoadAlerts(object sender, EventArgs e)
	{
		string address = Preferences.Default.Get(SettingsPage.selectedAddress, "");
		if (address == "")
		{
			await DisplayAlert("Cannot Use This Feature", "You have not selected a city to be alerted about. Please select one in Settings to be able to use this feature.", "Ok");
			return;
		}

		AlertLabel.Text = "Fetching Weather Data...";
		try
		{
			using var response = await WeatherRelay.Request($"alerts?now&address={address}");
			AlertLabel.Text = response.ToString();
		}
		catch (Exception ex)
		{
			AlertLabel.Text = ex.Message;
		}
	}
}