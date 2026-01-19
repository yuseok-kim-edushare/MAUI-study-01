using System.Net.Http.Json;
using System.Reflection;

namespace MauiStudyApp.Services;

/// <summary>
/// Implementation of API service for backend communication
/// This is a placeholder implementation - actual API endpoints should be configured
/// </summary>
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private string? _authToken;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        
        var baseUrl = ResolveBaseUrl();
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    public async Task<bool> AuthenticateAsync(string userId, string password)
    {
        try
        {
            // TODO: Implement actual authentication endpoint
            // Example implementation:
            var loginData = new { UserId = userId, Password = password };
            var response = await _httpClient.PostAsJsonAsync("/auth/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                _authToken = result?.Token;
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Authentication error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendDataAsync(string endpoint, object data)
    {
        try
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Send data error: {ex.Message}");
            return false;
        }
    }

    public async Task<T?> GetDataAsync<T>(string endpoint)
    {
        try
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            return default;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Get data error: {ex.Message}");
            return default;
        }
    }

    public async Task<bool> ValidateSessionAsync()
    {
        try
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync("/auth/validate");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Session validation error: {ex.Message}");
            return false;
        }
    }

    private void AddAuthHeader()
    {
        if (!string.IsNullOrEmpty(_authToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
        }
    }

    private class AuthResponse
    {
        public string? Token { get; set; }
    }

    private static string ResolveBaseUrl()
    {
        var baseUrl = GetAssemblyMetadata("ApiBaseUrl");
        if (!string.IsNullOrWhiteSpace(baseUrl))
        {
            return baseUrl;
        }

        return "https://your-backend-api.com/api";
    }

    private static string? GetAssemblyMetadata(string key)
    {
        return typeof(ApiService)
            .Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => string.Equals(a.Key, key, StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }
}
