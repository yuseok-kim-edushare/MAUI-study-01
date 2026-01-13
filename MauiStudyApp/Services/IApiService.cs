namespace MauiStudyApp.Services;

/// <summary>
/// Interface for API communication service
/// This provides a pipeline for connecting to backend server
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Authenticate user with backend
    /// </summary>
    Task<bool> AuthenticateAsync(string userId, string password);

    /// <summary>
    /// Send data to backend server
    /// </summary>
    Task<bool> SendDataAsync(string endpoint, object data);

    /// <summary>
    /// Get data from backend server
    /// </summary>
    Task<T?> GetDataAsync<T>(string endpoint);

    /// <summary>
    /// Check if user session is valid
    /// </summary>
    Task<bool> ValidateSessionAsync();
}
