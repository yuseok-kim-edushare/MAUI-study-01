# Quick Start Guide

This guide will help you get started with the MAUI Study App quickly.

## Prerequisites

Before you begin, ensure you have:

- ✅ .NET 10 SDK installed
- ✅ .NET MAUI workload installed
- ✅ Android SDK (API Level 21+)
- ✅ Android emulator or physical device

## Installation Steps

### 1. Clone the Repository

```bash
git clone https://github.com/yuseok-kim-edushare/MAUI-study-01.git
cd MAUI-study-01
```

### 2. Install .NET MAUI Workload (if not already installed)

```bash
dotnet workload install maui
```

### 3. Navigate to the Project

```bash
cd MauiStudyApp
```

### 4. Restore Dependencies

```bash
dotnet restore
```

### 5. Build the Project

```bash
dotnet build
```

## Running the App

### On Android Emulator

1. **Start an Android emulator** (or connect a physical device)
   
   ```bash
   # List available devices
   adb devices
   ```

2. **Run the app**
   
   ```bash
   dotnet build -t:Run -f net10.0-android
   ```

### First Launch

When you first launch the app:

1. You'll see the **Login Page**
2. Enter any User ID and Password (authentication is simulated)
3. Click **Login**
4. You'll be redirected to the **Home Page**

## Testing Features

### 1. Camera Feature

- Click the **"📷 Use Camera"** button
- Grant camera permission when prompted
- Take a photo to test camera functionality

### 2. Photo Picker Feature

- Click the **"🖼️ Pick Photo"** button
- Grant storage permission when prompted
- Select a photo from your gallery

### 3. GPS Location Feature

- Click the **"📍 Get GPS Location"** button
- Grant location permission when prompted
- View your current GPS coordinates

### 4. Background Service

The background service automatically:
- Maintains your session every 5 minutes
- Keeps the push notification pipeline active
- Status is displayed on the Home Page

## Project Structure Overview

```
MauiStudyApp/
│
├── Pages/                          # UI Pages
│   ├── LoginPage.xaml/.cs         # Login interface
│   └── HomePage.xaml/.cs          # Main app page
│
├── Services/                       # Business logic
│   ├── IApiService.cs             # API interface
│   ├── ApiService.cs              # API implementation
│   ├── IPushNotificationService.cs # Push notification interface
│   ├── PushNotificationService.cs  # Push implementation
│   └── BackgroundSessionService.cs # Session manager
│
├── Platforms/Android/              # Android-specific code
│   └── AndroidManifest.xml        # Permissions
│
└── Resources/                      # App resources
    ├── Images/                     # Image assets
    ├── Fonts/                      # Custom fonts
    └── Styles/                     # XAML styles
```

## Next Steps

### For Development

1. **Configure Backend API**
   - Update API URL in `Services/ApiService.cs`
   - See [IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md)

2. **Set Up Push Notifications**
   - Configure Firebase Cloud Messaging
   - See [IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md)

3. **Customize UI**
   - Modify colors in `Resources/Styles/Colors.xaml`
   - Update styles in `Resources/Styles/Styles.xaml`

### For Production

1. **Create a signing key**
   
   ```bash
   keytool -genkey -v -keystore myapp.keystore -alias myapp -keyalg RSA -keysize 2048 -validity 10000
   ```

2. **Build signed APK**
   
   ```bash
   dotnet publish -f net10.0-android -c Release \
     -p:AndroidSigningKeyStore=myapp.keystore \
     -p:AndroidSigningKeyAlias=myapp \
     -p:AndroidSigningStorePass=yourpassword \
     -p:AndroidSigningKeyPass=yourpassword
   ```

3. **Find the APK**
   
   The signed APK will be in:
   ```
   bin/Release/net10.0-android/publish/
   ```

## Troubleshooting

### Build Fails

**Problem**: Build errors or missing dependencies

**Solution**:
```bash
dotnet clean
dotnet restore
dotnet build
```

### App Crashes on Start

**Problem**: App crashes immediately after launch

**Solution**:
- Check Android logcat: `adb logcat | grep -i "maui"`
- Ensure minimum Android API level is 21+
- Verify all NuGet packages are restored

### Permissions Not Working

**Problem**: Camera/Location/Storage features don't work

**Solution**:
- Check `AndroidManifest.xml` has correct permissions
- Ensure you're testing on Android 6.0+ (API 23+)
- Grant permissions manually in device settings

### Background Service Stops

**Problem**: Session validation stops after some time

**Solution**:
- Disable battery optimization for the app
- Consider implementing a foreground service (see IMPLEMENTATION_GUIDE.md)

## Getting Help

- 📖 Read the [README.md](../README.md) for project overview
- 📖 Check [IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md) for detailed integration steps
- 📖 See [CONFIGURATION.md](CONFIGURATION.md) for configuration options
- 🐛 Report issues on GitHub

## Learning Resources

- [.NET MAUI Documentation](https://docs.microsoft.com/en-us/dotnet/maui/)
- [XAML Basics](https://docs.microsoft.com/en-us/dotnet/maui/xaml/)
- [Android Developer Guide](https://developer.android.com/)
- [Firebase Cloud Messaging](https://firebase.google.com/docs/cloud-messaging)

## License

This project is licensed under the MIT License.
