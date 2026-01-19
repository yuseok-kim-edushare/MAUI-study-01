namespace MauiStudyBackend.Models;

public sealed class DeviceRegistration
{
    public string DeviceToken { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
}
