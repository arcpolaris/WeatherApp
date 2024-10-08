﻿using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace WeatherApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
		.UseMauiApp<App>().ConfigureFonts(fonts =>
		{
			fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
		}).UseMauiCommunityToolkit();
		builder.Services.AddSingleton<WeatherRelaySocketService>();
#if DEBUG
		builder.Logging.AddDebug();
#endif
		return builder.Build();
	}
}