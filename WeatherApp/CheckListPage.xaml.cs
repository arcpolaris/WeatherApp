using System.Collections.ObjectModel;

namespace WeatherApp;

public partial class CheckListPage : ContentPage
{
    public ObservableCollection<CheckListItem> Items { get; set; }

    private static readonly string[] items = [
            "Backpack",
            "Batteries",
            "Flashlight",
            "First Aid Kit",
            "Non-perishable Food",
            "Water (At Least a Gallon per Person)",
            "Paper and Writing Implements",
            "Whistle",
            "Paper Towels",
            "Tissues",
            "Toilet Paper",
            "Battery Powered NOAA Radio",
            "Fire Extinguisher",
            "Mechanical Cell Phone Charger"
        ];

    public CheckListPage()
    {
        InitializeComponent();

        Items = new(items.Select(name => new CheckListItem(name)));

        BindingContext = this;
    }

    public class CheckListItem(string name)
    {
        private readonly string _name = name;

        private string Key => $"{GetType()}:{_name}";

        public string Name => _name;

        public bool IsChecked
        {
            get => Preferences.Default.Get(Key, false);
            set => Preferences.Default.Set(Key, value);
        }
    }
}