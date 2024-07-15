using System.Collections.ObjectModel;
using static System.Tuple;

namespace WeatherApp;

public partial class MapsPage : ContentPage
{
	public ObservableCollection<Tuple<string, string>> Queries { get; set; } = [];
	readonly static List<Tuple<string, string>> _queries = [
		Create("Hospitals Near Me", "Hospital"),
		Create("Vets Near Me", "Veterinarian"),
		Create("Car Repair", "Mechanic"),
		Create("Insurance Near Me", "Insurance Service"),
		Create("Roofers" , "Roofers"),
		Create("Siding Services", "Siding, Service")
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