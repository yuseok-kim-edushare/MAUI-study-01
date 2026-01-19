# App Configuration

This file contains configuration options for the MAUI Study App.

## Environment Configuration

> **Note**: The app reads `ApiBaseUrl` from build-time assembly metadata. Provide it with
> `-p:ApiBaseUrl=https://your-backend-api.com/api` when building. The JSON samples below
> are conceptual examples only.

### Development
```json
{
  "ApiBaseUrl": "https://dev-api.example.com/api",
  "PushNotificationEnabled": true,
  "SessionCheckInterval": 300,
  "LogLevel": "Debug"
}
```

### Staging
```json
{
  "ApiBaseUrl": "https://staging-api.example.com/api",
  "PushNotificationEnabled": true,
  "SessionCheckInterval": 300,
  "LogLevel": "Information"
}
```

### Production
```json
{
  "ApiBaseUrl": "https://api.example.com/api",
  "PushNotificationEnabled": true,
  "SessionCheckInterval": 300,
  "LogLevel": "Warning"
}
```

## Feature Flags

You can enable/disable features by modifying the service registration in `MauiProgram.cs`:

```csharp
// Enable/disable background service
builder.Services.AddSingleton<BackgroundSessionService>();

// Enable/disable push notifications
builder.Services.AddSingleton<IPushNotificationService, PushNotificationService>();

// Enable/disable API service
builder.Services.AddSingleton<IApiService, ApiService>();
```

## Android Specific Configuration

### Debug Settings
For development builds, you can enable additional logging:

```xml
<!-- Add to MauiStudyApp.csproj -->
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
  <AndroidEnableMultiDex>true</AndroidEnableMultiDex>
  <AndroidLinkMode>None</AndroidLinkMode>
  <EmbedAssembliesIntoApk>false</EmbedAssembliesIntoApk>
</PropertyGroup>
```

### Release Settings
For production builds:

```xml
<!-- Add to MauiStudyApp.csproj -->
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidEnableMultiDex>true</AndroidEnableMultiDex>
  <AndroidLinkMode>Full</AndroidLinkMode>
  <EmbedAssembliesIntoApk>true</EmbedAssembliesIntoApk>
  <AndroidPackageFormat>apk</AndroidPackageFormat>
  <AndroidUseAapt2>true</AndroidUseAapt2>
  <AndroidCreatePackagePerAbi>false</AndroidCreatePackagePerAbi>
</PropertyGroup>
```

## Session Configuration

Modify session check interval in `BackgroundSessionService.cs`:

```csharp
// Check session every 5 minutes (default)
private static readonly TimeSpan SessionCheckInterval = TimeSpan.FromMinutes(5);

// Or customize for your needs:
// private static readonly TimeSpan SessionCheckInterval = TimeSpan.FromMinutes(10);
```

## API Timeout Configuration

Adjust HTTP client timeout in `MauiProgram.cs`:

```csharp
builder.Services.AddSingleton<HttpClient>(sp =>
{
    var httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(30) // Adjust as needed
    };
    return httpClient;
});
```

## Permission Configuration

Customize which permissions are requested by modifying `AndroidManifest.xml`:

```xml
<!-- Optional: Remove permissions you don't need -->
<!-- <uses-permission android:name="android.permission.CAMERA" /> -->
<!-- <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" /> -->
```

## UI Customization

### Theme Colors
Modify colors in `Resources/Styles/Colors.xaml`:

```xml
<Color x:Key="Primary">#512BD4</Color>
<Color x:Key="Secondary">#DFD8F7</Color>
<!-- Add your custom colors -->
```

### Font Sizes
Modify styles in `Resources/Styles/Styles.xaml`:

```xml
<Style x:Key="Headline" TargetType="Label">
    <Setter Property="FontSize" Value="32" />
    <!-- Customize as needed -->
</Style>
```

## Build Configuration

### Building for Different Architectures

```bash
# Build for ARM64 only
dotnet build -f net10.0-android -r android-arm64

# Build for x64 (emulator)
dotnet build -f net10.0-android -r android-x64

# Build for all architectures
dotnet build -f net10.0-android
```

### Creating Release APK

```bash
# Create signed APK
dotnet publish -f net10.0-android -c Release -p:AndroidSigningKeyStore=myapp.keystore -p:AndroidSigningKeyAlias=myapp -p:AndroidSigningStorePass=password -p:AndroidSigningKeyPass=password
```

## Debugging Tips

### Enable Verbose Logging

Add to `MauiProgram.cs`:

```csharp
#if DEBUG
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddDebug();
builder.Logging.AddConsole();
#endif
```

### View Debug Output

Use Android logcat to view debug messages:

```bash
adb logcat | grep -i "maui"
```

### Remote Debugging

Enable USB debugging on your Android device:
1. Settings → About Phone → Tap "Build Number" 7 times
2. Settings → Developer Options → Enable USB Debugging
3. Connect device via USB
4. Run: `dotnet build -t:Run -f net10.0-android`
