using Plugin.LocalNotification;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace WeatherApp;

public partial class HomePage : ContentPage
{
	public ObservableCollection<NWSAlert> NWSAlerts { get; set; }
	public ObservableCollection<NotificationRequest> SirenAlerts { get; set; }

	public HomePage()
	{
		InitializeComponent();
		NWSAlerts = new();
		SirenAlerts = new();
		BindingContext = this;
	}

	protected override void OnAppearing()
	{
		App.relayService!.Notified += (sender, e) => { SirenAlerts.Add(e); };
		base.OnAppearing();
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
			var response = await WeatherRelay.Request<Feature[]>($"alerts?now&address={address}");
			var features = response.Value ?? [];
			if (features.Length == 0)
			{
				AlertLabel.Text = "No active NWS alerts.";
				return;
			}
			AlertLabel.Text = "Active NWS alert(s):";
			NWSAlerts.Clear();
			foreach (var feature in features)
			{
				NWSAlerts.Add(feature);
			}
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

	public class NWSAlert
	{
		public string Location { get; set; }
		public string Reporter { get; set; }
		public string Headline { get; set; }
		public string Subtitle { get; set; }
		public string Maintext { get; set; }
		public string Instruct { get; set; }
		public string Starting { get; set; }
		public string Stoptime { get; set; }

		public NWSAlert(string location, string reporter, string headline, string subtitle, string maintext, string instruct, string starting, string stoptime)
		{
			Location = location;
			Reporter = reporter;
			Headline = headline;
			Subtitle = subtitle;
			Maintext = maintext;
			Instruct = instruct;
			Starting = starting;
			Stoptime = stoptime;
		}

		public static implicit operator NWSAlert(Feature feature)
		{
			string location = feature.properties.areaDesc;
			string reporter = feature.properties.senderName;
			string headline = feature.properties.parameters.NWSheadline[0];
			string subtitle = feature.properties.headline;
			string maintext = feature.properties.description;
			string instruct = feature.properties.instruction;

			string starting = DateTime.Parse(feature.properties.onset, null, System.Globalization.DateTimeStyles.RoundtripKind).ToString("f");
			string stoptime = DateTime.Parse(feature.properties.ends, null, System.Globalization.DateTimeStyles.RoundtripKind).ToString("f");
			return new(location, reporter, headline, subtitle, maintext, instruct, starting, stoptime);
		}
	}
}