using Plugin.LocalNotification;

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

	async void TestNotif(object sender, EventArgs e)
	{
		NotificationRequest request = new()
		{
			NotificationId = 1000,
			Title = "Title",
			Subtitle = "Subtitle",
			Description = "Description",
			BadgeNumber = 42,
			Schedule = new()
			{
				NotifyTime = DateTime.Now.AddSeconds(3),
				NotifyRepeatInterval = TimeSpan.FromDays(1)
			}
		};
		if (!await LocalNotificationCenter.Current.AreNotificationsEnabled())
			await LocalNotificationCenter.Current.RequestNotificationPermission();
		await LocalNotificationCenter.Current.Show(request);
	}
}