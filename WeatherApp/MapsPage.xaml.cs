using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using static System.Tuple;

namespace WeatherApp;

public partial class MapsPage : ContentPage
{
	public ObservableCollection<Tuple<string, string>> Queries { get; set; } = [];
	readonly static List<Tuple<string, string>> _queries = [
		Create("Hospitals", "Hospital"),
		Create("Veterinarians", "Veterinarian"),
		Create("Insurance", "Insurance Service"),
		Create("Car Repair", "Mechanic"),
		Create("Roofers" , "Roofers"),
		Create("Siding Services", "Siding, Service"),
		Create("Tree Services", "Tree Services")
	];
	public Command<string> MapCommand { get; set; } = new(OpenMap);

	public MapsPage()
	{
		InitializeComponent();
		BindingContext = this;
		_queries.ForEach(item => Queries.Add(item));
	}

	public static async void OpenMap(string query)
	{
		Uri uri = new($"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(query)}");
		await Launcher.OpenAsync(uri);
	}
}