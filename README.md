# MAUI-study-01

.NET MAUI study project for mobile app development - demonstrating basic mobile app features with push notifications, background services, and device capabilities.

## Platform Support

| Platform | Target/Build Version | Minimum Support | Notes |
| --- | --- | --- | --- |
| **Android** | API 36 (Android 15) | API 30 (Android 11) | 16KB page size support enabled |
| **iOS** | iOS 26 SDK (Xcode 26) | iOS 17.0 | Latest Swift API and security frameworks |

## Features

This project demonstrates:

### 1. User Authentication
- **Login Page**: Clean UI with ID/password authentication
- **Secure Storage**: Uses MAUI SecureStorage for credential management
- **Session Management**: Maintains user session across app restarts

### 2. Push Notification Pipeline
- **IPushNotificationService**: Interface for receiving alerts from backend server
- **ASP.NET Core Integration**: Ready for SignalR or native push notification integration
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
- **ASP.NET Core Backend Support**: Ready for integration with ASP.NET Core Web API
- **Authentication Flow**: Supports JWT token-based authentication
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
│   ├── Android/
│   │   └── AndroidManifest.xml # Android permissions (API 30-36)
│   └── iOS/
│       └── Info.plist          # iOS permissions (iOS 17.0+)
└── MauiProgram.cs             # App startup and DI configuration
```

## Platform-Specific Configurations

### Android (API 30-36 with 16KB Page Support)

The app is configured for:
- **Minimum SDK**: API 30 (Android 11)
- **Target SDK**: API 36 (Android 15)
- **16KB Page Size**: Enabled for Google Play compliance
- **Foreground Service**: Data sync type for background session

Permissions configured:
- Internet & Network, Camera, Storage, Location
- Wake Lock, Push Notifications
- Foreground Service (for background session maintenance)

### iOS (iOS 17.0+)

The app is configured for:
- **Minimum iOS Version**: 17.0
- **Target SDK**: iOS 26 SDK (Xcode 26)
- **Background Modes**: Fetch and remote notifications

Permissions configured:
- Camera usage, Photo library access, Location services
- Background modes for session maintenance

## Getting Started

### Prerequisites
- .NET 10 SDK or later
- .NET MAUI workload installed (`dotnet workload install maui`)
- Android SDK (for Android development)
- Xcode 26 (for iOS development on macOS)

### Build and Run

```bash
# Navigate to project directory
cd MauiStudyApp

# Restore dependencies
dotnet restore

# Fast build on Windows (Android) with SDK paths and API base URL
dotnet build -f net10.0-android -c Debug \
    -p:AndroidSdkDirectory=c:\android-sdk \
    -p:JavaSdkDirectory="C:\Program Files\Eclipse Adoptium\jdk-21.0.9.10-hotspot" \
    -p:ApiBaseUrl=https://your-backend-api.com/api

# Run on Android device/emulator
dotnet build -t:Run -f net10.0-android

# Run on iOS simulator (macOS only)
dotnet build -t:Run -f net10.0-ios
```

## Configuration

### API Backend URL
Pass the backend API URL at build time using MSBuild property `ApiBaseUrl`:

```bash
dotnet build -f net10.0-android -c Debug \
    -p:ApiBaseUrl=https://your-backend-api.com/api
```

The app reads this value from assembly metadata at runtime (see `ApiService.ResolveBaseUrl()`). If not provided, it falls back to `https://your-backend-api.com/api`.

### Push Notifications
To enable push notifications with ASP.NET Core backend:
1. **Option 1**: Use SignalR for real-time push (recommended for cross-platform)
2. **Option 2**: Use native push (APNs for iOS, FCM for Android) via backend
3. Implement device token registration in `PushNotificationService.cs`
4. Configure notification handling in `BackgroundSessionService.cs`

See [IMPLEMENTATION_GUIDE.md](MauiStudyApp/IMPLEMENTATION_GUIDE.md) for detailed instructions.

## Backend Integration

This app is designed to work with an ASP.NET Core backend. See the implementation guide for:
- Setting up ASP.NET Core Web API
- JWT authentication configuration
- SignalR real-time notifications
- Device token management

## Future Enhancements

- [ ] Complete SignalR/native push notification integration
- [ ] Implement real ASP.NET Core backend API
- [ ] Add data caching for offline support
- [ ] Implement biometric authentication
- [ ] Add more device capabilities (contacts, SMS, etc.)
- [ ] Implement token refresh mechanism

## License

This project is licensed under the MIT License - see the LICENSE file for details.
