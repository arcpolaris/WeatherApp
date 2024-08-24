using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace WeatherApp;

public partial class EmergencyPage : ContentPage
{
	//static readonly string url = "https://drive.google.com/file/d/14rYgRmPwjvWvQ97BPJ8_wazbjz2-KhVP/preview";

	public ICommand OpenPage => new Command(async () => await Launcher.OpenAsync(@"https://www.weather.gov/mkx/nwr-table"));
	public EmergencyPage()
	{
		InitializeComponent();
		BindingContext = this;
		//pdfWebView.Source = url;
		//pdfWebView.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>()
		//	.SetDisplayZoomControls(true)
		//	.SetEnableZoomControls(true);
	}
}