using Microsoft.Extensions.DependencyInjection;
using MauiStudyApp.Services;

namespace MauiStudyApp;

public partial class App : Application
{
	private readonly BackgroundSessionService _backgroundService;

	public App(BackgroundSessionService backgroundService)
	{
		InitializeComponent();
		_backgroundService = backgroundService;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}

	protected override async void OnStart()
	{
		base.OnStart();

		// Check if user is already logged in
		var isLoggedIn = await SecureStorage.GetAsync("is_logged_in");
		if (isLoggedIn == "true")
		{
			// Navigate to home page
			await Shell.Current.GoToAsync("//main");
			
			// Start background service
			await _backgroundService.StartAsync();
		}
		else
		{
			// Navigate to login page
			await Shell.Current.GoToAsync("//login");
		}
	}

	protected override async void OnSleep()
	{
		base.OnSleep();
		// Background service continues to run
	}

	protected override async void OnResume()
	{
		base.OnResume();
		// Ensure background service is running if logged in
		var isLoggedIn = await SecureStorage.GetAsync("is_logged_in");
		if (isLoggedIn == "true")
		{
			await _backgroundService.StartAsync();
		}
	}
}