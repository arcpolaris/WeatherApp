using System.Text.Json;

namespace WeatherApp;

public partial class SettingsPage : ContentPage
{
	Dictionary<string, IEnumerable<string>> cityLookup;

	public SettingsPage()
	{
		InitializeComponent();

		countyPicker.SelectedIndex = -1;
		cityPicker.SelectedIndex = -1;

		cityLookup = GenerateCityLookup().Result;
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
		cityPicker.ItemsSource = cityLookup[(string)picker.ItemsSource[selectedIndex]!].ToList();
	}

	static async Task<JsonDocument> LoadSirenData()
	{
		using var stream = await FileSystem.OpenAppPackageFileAsync("SirenData.json");
		//using var reader = new StreamReader(stream);

		JsonDocument doc = await JsonDocument.ParseAsync(stream);
		return doc;
	}

	static async Task<Dictionary<string, IEnumerable<string>>> GenerateCityLookup()
	{
		using var doc = await LoadSirenData();
		var cityRoot = doc.RootElement.GetProperty("cities");
		return new(
			cityRoot.EnumerateObject()
				.Select(property => new KeyValuePair<string, IEnumerable<string>>(
					property.Name,
					property.Value.EnumerateArray()
						.Select(element => element.GetString()!)
				))
		);
	}
}