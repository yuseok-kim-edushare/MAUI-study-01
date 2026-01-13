namespace MauiStudyApp.Pages;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        LoadUserInfo();
    }

    private async void LoadUserInfo()
    {
        try
        {
            var userId = await SecureStorage.GetAsync("user_id");
            if (!string.IsNullOrEmpty(userId))
            {
                WelcomeLabel.Text = $"Welcome, {userId}!";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to load user info: {ex.Message}", "OK");
        }
    }

    private async void OnCameraClicked(object sender, EventArgs e)
    {
        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    await DisplayAlertAsync("Success", $"Photo captured: {photo.FileName}", "OK");
                }
            }
            else
            {
                await DisplayAlertAsync("Not Supported", "Camera is not supported on this device", "OK");
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlertAsync("Not Supported", "Camera is not supported on this device", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlertAsync("Permission Denied", "Camera permission is required", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to use camera: {ex.Message}", "OK");
        }
    }

    private async void OnPhotoClicked(object sender, EventArgs e)
    {
        try
        {
            var results = await MediaPicker.Default.PickPhotosAsync();
            if (results != null && results.Any())
            {
                await DisplayAlertAsync("Success", $"Photo selected: {results.First().FileName}", "OK");
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlertAsync("Not Supported", "Photo picker is not supported on this device", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlertAsync("Permission Denied", "Storage permission is required", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to pick photo: {ex.Message}", "OK");
        }
    }

    private async void OnLocationClicked(object sender, EventArgs e)
    {
        try
        {
            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(30)
            });

            if (location != null)
            {
                await DisplayAlertAsync("Location",
                    $"Latitude: {location.Latitude}\nLongitude: {location.Longitude}\nAltitude: {location.Altitude}",
                    "OK");
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlertAsync("Not Supported", "GPS is not supported on this device", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlertAsync("Permission Denied", "Location permission is required", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Failed to get location: {ex.Message}", "OK");
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (confirm)
        {
            // Clear stored credentials
            SecureStorage.RemoveAll();

            // Navigate to login page
            await Shell.Current.GoToAsync("//login");
        }
    }
}
