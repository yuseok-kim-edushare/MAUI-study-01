namespace MauiStudyApp.Services;

/// <summary>
/// Interface for push notification service
/// This provides a pipeline for receiving alerts from backend
/// </summary>
public interface IPushNotificationService
{
    /// <summary>
    /// Initialize push notification service
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Register device for push notifications
    /// </summary>
    Task<bool> RegisterDeviceAsync(string deviceToken);

    /// <summary>
    /// Unregister device from push notifications
    /// </summary>
    Task<bool> UnregisterDeviceAsync();

    /// <summary>
    /// Handle received push notification
    /// </summary>
    void HandleNotification(string title, string message, Dictionary<string, string> data);

    /// <summary>
    /// Event fired when notification is received
    /// </summary>
    event EventHandler<NotificationEventArgs>? NotificationReceived;
}

/// <summary>
/// Event args for notification received event
/// </summary>
public class NotificationEventArgs : EventArgs
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string> Data { get; set; } = new();
}
