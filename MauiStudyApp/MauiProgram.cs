using Microsoft.Extensions.Logging;
using MauiStudyApp.Services;
using MauiStudyApp.Pages;

namespace MauiStudyApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Register services
		builder.Services.AddSingleton<HttpClient>();
		builder.Services.AddSingleton<IApiService, ApiService>();
		builder.Services.AddSingleton<IPushNotificationService, PushNotificationService>();
		builder.Services.AddSingleton<BackgroundSessionService>();

		// Register pages
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<HomePage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
