# MAUI-study-01

.NET MAUI study project for mobile app development - demonstrating basic mobile app features with push notifications, background services, and device capabilities.

## Features

This project demonstrates:

### 1. User Authentication
- **Login Page**: Clean UI with ID/password authentication
- **Secure Storage**: Uses MAUI SecureStorage for credential management
- **Session Management**: Maintains user session across app restarts

### 2. Push Notification Pipeline
- **IPushNotificationService**: Interface for receiving alerts from backend server
- **Notification Handling**: Pipeline ready for Firebase Cloud Messaging integration
- **Event-based Architecture**: Notification events can trigger UI updates

### 3. Background Services
- **Background Session Service**: Maintains user session while app runs
- **Periodic Session Validation**: Checks session validity every 5 minutes
- **Automatic Session Maintenance**: Keeps connection alive with backend

### 4. Device Capabilities (Foreground Only)
When the app is in the foreground, users can access:
- **Camera**: Capture photos using device camera
- **Photo Picker**: Select photos from device gallery
- **GPS/Location**: Get current device location coordinates

### 5. API Communication Pipeline
- **IApiService**: Interface for backend API communication
- **RESTful API Support**: Ready for integration with backend server
- **Authentication Flow**: Supports token-based authentication
- **Data Exchange**: Send and receive data from backend

## Project Structure

```
MauiStudyApp/
├── Pages/
│   ├── LoginPage.xaml/.cs      # User login interface
│   └── HomePage.xaml/.cs        # Main app page with feature access
├── Services/
│   ├── IApiService.cs          # API communication interface
│   ├── ApiService.cs           # API service implementation
│   ├── IPushNotificationService.cs  # Push notification interface
│   ├── PushNotificationService.cs   # Push notification implementation
│   └── BackgroundSessionService.cs  # Background session manager
├── Platforms/
│   └── Android/
│       └── AndroidManifest.xml # Android permissions configuration
└── MauiProgram.cs             # App startup and DI configuration
```

## Android Permissions

The app requests the following permissions (configured in `AndroidManifest.xml`):

- **Internet & Network**: For API communication
- **Camera**: For capturing photos
- **Storage**: For accessing photo gallery
- **Location**: For GPS functionality
- **Wake Lock**: For background service operation
- **Push Notifications**: For receiving alerts from backend

## Getting Started

### Prerequisites
- .NET 10 SDK or later
- .NET MAUI workload installed (`dotnet workload install maui-android`)
- Android SDK (for Android development)

### Build and Run

```bash
# Navigate to project directory
cd MauiStudyApp

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run on Android device/emulator
dotnet build -t:Run -f net10.0-android
```

## Configuration

### API Backend URL
Update the backend API URL in `Services/ApiService.cs`:
```csharp
private const string BaseUrl = "https://your-backend-api.com/api";
```

### Push Notifications
To enable push notifications:
1. Set up Firebase Cloud Messaging for Android
2. Implement device token registration in `PushNotificationService.cs`
3. Configure notification handling in `BackgroundSessionService.cs`

## Future Enhancements

- [ ] Implement actual Firebase Cloud Messaging integration
- [ ] Add iOS support (requires iOS workload)
- [ ] Implement real API backend integration
- [ ] Add data caching for offline support
- [ ] Implement biometric authentication
- [ ] Add more device capabilities (contacts, SMS, etc.)

## License

This project is licensed under the MIT License - see the LICENSE file for details.
