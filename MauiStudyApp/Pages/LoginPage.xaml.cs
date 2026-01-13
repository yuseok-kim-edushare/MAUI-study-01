namespace MauiStudyApp.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string? userId = UserIdEntry.Text?.Trim();
        string? password = PasswordEntry.Text;

        // Reset status
        StatusLabel.IsVisible = false;
        StatusLabel.Text = "";

        // Basic validation
        if (string.IsNullOrWhiteSpace(userId))
        {
            StatusLabel.Text = "Please enter your User ID";
            StatusLabel.IsVisible = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            StatusLabel.Text = "Please enter your Password";
            StatusLabel.IsVisible = true;
            return;
        }

        // Disable login button while processing
        LoginButton.IsEnabled = false;
        LoginButton.Text = "Logging in...";

        try
        {
            // TODO: Implement actual authentication via API
            // For now, simulate authentication
            await Task.Delay(1000); // Simulate network delay

            // Store login state (in production, use secure storage)
            await SecureStorage.SetAsync("user_id", userId);
            await SecureStorage.SetAsync("is_logged_in", "true");

            // Navigate to main page
            await Shell.Current.GoToAsync("//main");
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Login failed: {ex.Message}";
            StatusLabel.IsVisible = true;
        }
        finally
        {
            LoginButton.IsEnabled = true;
            LoginButton.Text = "Login";
        }
    }
}
