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

		await MainThread.InvokeOnMainThreadAsync(async () =>
		{
			try
			{
				// Check if user is already logged in
				var isLoggedIn = await GetIsLoggedInAsync();
				if (isLoggedIn)
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
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Startup navigation error: {ex.Message}");
				// Fall back to login page on any startup error
				await Shell.Current.GoToAsync("//login");
			}
		});
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
		if (await GetIsLoggedInAsync())
		{
			await _backgroundService.StartAsync();
		}
	}

	private static async Task<bool> GetIsLoggedInAsync()
	{
		try
		{
			var isLoggedIn = await SecureStorage.GetAsync("is_logged_in");
			if (string.Equals(isLoggedIn, "true", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"SecureStorage read failed: {ex.Message}");
		}

		// Fallback for devices without secure lock screen
		return Preferences.Get("is_logged_in", false);
	}
}