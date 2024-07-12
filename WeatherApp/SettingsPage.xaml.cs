using System.Text.Json;

namespace WeatherApp;

public partial class SettingsPage : ContentPage
{
	public static readonly string selectedAddress = nameof(selectedAddress);
	Dictionary<string, List<string>>? cityLookup;

	public SettingsPage()
	{
		InitializeComponent();

		LoadCityLookup();
	}

	async void LoadCityLookup()
	{
		cityLookup = await GenerateCityLookup();
		countyPicker.ItemsSource = cityLookup.Keys.ToList();
		countyPicker.SelectedIndexChanged += CountyPicker_SelectedIndexChanged;

		countyPicker.SelectedIndex = Preferences.Default.Get($"{nameof(cityLookup)}:{nameof(countyPicker)}", -1);
		CountyPicker_SelectedIndexChanged(countyPicker, EventArgs.Empty);
		cityPicker.SelectedIndex = Preferences.Default.Get($"{nameof(cityLookup)}:{nameof(cityPicker)}", -1);
	}

	void CountyPicker_SelectedIndexChanged(object? sender, EventArgs e)
	{
		var picker = (Picker)sender!;
		int selectedIndex = picker.SelectedIndex;
		if (selectedIndex == -1) return;
		if (cityLookup is null) return;
		cityPicker.ItemsSource = cityLookup[(string)picker.ItemsSource[selectedIndex]!];
		cityPicker.SelectedIndex = -1;
	}

	async void ApplyPrefs(object? sender, EventArgs e)
	{
		SettingState state = SettingState.Valid;
		if (cityLookup is null) state = SettingState.Loading;
		else if (cityPicker.SelectedIndex == -1 || countyPicker.SelectedIndex == -1) state = SettingState.Missing;

		string message;
		switch (state)
		{
			case SettingState.Valid:
				Preferences.Default.Set($"{nameof(cityLookup)}:{nameof(cityPicker)}", cityPicker.SelectedIndex);
				Preferences.Default.Set($"{nameof(cityLookup)}:{nameof(countyPicker)}", countyPicker.SelectedIndex);
				Preferences.Default.Set($"{nameof(selectedAddress)}", $"{cityPicker.SelectedItem}, {countyPicker.SelectedItem}, Wisconsin, USA");
				message = "Settings saved successfully.";
				break;
			case SettingState.Loading:
				message = "Internal resources are still loading.";
				break;
			case SettingState.Missing:
				message = "One or more fields are empty.";
				break;
			default:
				message = "An unknown error occured.";
				break;
		}
		await DisplayAlert(state is SettingState.Valid ? "Saved Settings" : "Could Not Save Settings", message, "Ok");
	}

	static async Task<Dictionary<string, List<string>>> GenerateCityLookup()
	{
		using var stream = await FileSystem.OpenAppPackageFileAsync("SirenData.json");
		using var doc = await JsonDocument.ParseAsync(stream);
		var cityRoot = doc.RootElement.GetProperty("cities");
		return new(
		cityRoot.EnumerateObject()
			.ToDictionary(
				property => property.Name,
				property => property.Value.EnumerateArray()
					.Select(element => element.GetString()!)
					.ToList()
			));
	}

	enum SettingState
	{
		Valid,
		Missing,
		Loading,
		Invalid,
	}
}