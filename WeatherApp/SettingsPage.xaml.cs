using System.Text.Json;

namespace WeatherApp;

public partial class SettingsPage : ContentPage
{
	Dictionary<string, List<string>>? cityLookup;

	public SettingsPage()
	{
		InitializeComponent();

		countyPicker.SelectedIndex = -1;
		cityPicker.SelectedIndex = -1;
		LoadCityLookup();
	}

	async void LoadCityLookup()
	{
		cityLookup = await GenerateCityLookup();
		countyPicker.ItemsSource = cityLookup.Keys.ToList();
		countyPicker.SelectedIndexChanged += CountyPicker_SelectedIndexChanged;
		cityPicker.SelectedIndexChanged += CityPicker_SelectedIndexChanged;
	}

	void CityPicker_SelectedIndexChanged(object? sender, EventArgs e)
	{
		//throw new NotImplementedException();
	}

	void CountyPicker_SelectedIndexChanged(object? sender, EventArgs e)
	{
		var picker = (Picker)sender!;
		int selectedIndex = picker.SelectedIndex;
		if (selectedIndex == -1) return;
		if (cityLookup == null) return;
		cityPicker.ItemsSource = cityLookup[(string)picker.ItemsSource[selectedIndex]!];
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
}