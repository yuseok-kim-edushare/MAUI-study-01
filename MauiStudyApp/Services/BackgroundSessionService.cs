namespace MauiStudyApp.Services;

/// <summary>
/// Background service for maintaining user session and receiving notifications
/// This service runs in the background to keep the session alive
/// </summary>
public class BackgroundSessionService
{
    private readonly IApiService _apiService;
    private readonly IPushNotificationService _pushNotificationService;
    private PeriodicTimer? _sessionTimer;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isRunning;

    // Session check interval (5 minutes)
    private static readonly TimeSpan SessionCheckInterval = TimeSpan.FromMinutes(5);

    public BackgroundSessionService(
        IApiService apiService,
        IPushNotificationService pushNotificationService)
    {
        _apiService = apiService;
        _pushNotificationService = pushNotificationService;
    }

    /// <summary>
    /// Start the background service
    /// </summary>
    public async Task StartAsync()
    {
        if (_isRunning)
        {
            return;
        }

        _isRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();

        // Initialize push notification service
        await _pushNotificationService.InitializeAsync();

        // Subscribe to notification events
        _pushNotificationService.NotificationReceived += OnNotificationReceived;

        // Start session maintenance timer
        _sessionTimer = new PeriodicTimer(SessionCheckInterval);

        // Run background task
        _ = Task.Run(async () => await RunBackgroundTaskAsync(_cancellationTokenSource.Token));

        System.Diagnostics.Debug.WriteLine("Background session service started");
    }

    /// <summary>
    /// Stop the background service
    /// </summary>
    public async Task StopAsync()
    {
        if (!_isRunning)
        {
            return;
        }

        _isRunning = false;
        _cancellationTokenSource?.Cancel();
        _sessionTimer?.Dispose();

        // Unsubscribe from notification events
        _pushNotificationService.NotificationReceived -= OnNotificationReceived;

        await Task.CompletedTask;
        System.Diagnostics.Debug.WriteLine("Background session service stopped");
    }

    private async Task RunBackgroundTaskAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (await _sessionTimer!.WaitForNextTickAsync(cancellationToken))
            {
                // Validate and maintain session
                var isValid = await _apiService.ValidateSessionAsync();

                if (!isValid)
                {
                    System.Diagnostics.Debug.WriteLine("Session expired or invalid");
                    // TODO: Handle session expiration (notify user, redirect to login, etc.)
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Session is valid");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Normal cancellation
            System.Diagnostics.Debug.WriteLine("Background task cancelled");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Background task error: {ex.Message}");
        }
    }

    private void OnNotificationReceived(object? sender, NotificationEventArgs e)
    {
        // Handle notification in the app
        System.Diagnostics.Debug.WriteLine($"App received notification - Title: {e.Title}, Message: {e.Message}");
        
        // TODO: Show in-app notification, update UI, etc.
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Update UI or show notification to user
        });
    }
}
