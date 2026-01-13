# Implementation Guide

This guide provides detailed instructions for implementing the backend API integration and push notification features.

## Backend API Integration

### Setting Up Your Backend API

1. **Update API Base URL**
   
   Edit `MauiStudyApp/Services/ApiService.cs`:
   ```csharp
   private const string BaseUrl = "https://your-backend-api.com/api";
   ```

2. **Implement Authentication Endpoint**

   Your backend should provide an authentication endpoint:
   ```
   POST /auth/login
   Request: { "userId": "string", "password": "string" }
   Response: { "token": "string" }
   ```

3. **Session Validation Endpoint**

   Implement an endpoint to validate active sessions:
   ```
   GET /auth/validate
   Headers: Authorization: Bearer {token}
   Response: 200 OK or 401 Unauthorized
   ```

4. **Using the API Service**

   The `IApiService` interface provides methods for:
   - `AuthenticateAsync()` - User authentication
   - `SendDataAsync()` - Send data to backend
   - `GetDataAsync<T>()` - Retrieve data from backend
   - `ValidateSessionAsync()` - Check session validity

## Push Notification Setup

### Firebase Cloud Messaging (Android)

1. **Create Firebase Project**
   - Go to [Firebase Console](https://console.firebase.google.com/)
   - Create a new project
   - Add Android app to your project
   - Download `google-services.json`

2. **Add NuGet Packages**
   ```bash
   dotnet add package Xamarin.Firebase.Messaging
   dotnet add package Xamarin.GooglePlayServices.Base
   ```

3. **Configure Android Project**

   Add to `MauiStudyApp.csproj`:
   ```xml
   <ItemGroup>
     <GoogleServicesJson Include="google-services.json" />
   </ItemGroup>
   ```

4. **Implement Firebase Messaging Service**

   Create `Platforms/Android/MyFirebaseMessagingService.cs`:
   ```csharp
   using Android.App;
   using Firebase.Messaging;
   using MauiStudyApp.Services;

   [Service(Exported = true)]
   [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
   public class MyFirebaseMessagingService : FirebaseMessagingService
   {
       public override void OnMessageReceived(RemoteMessage message)
       {
           base.OnMessageReceived(message);
           
           var notificationService = MauiApplication.Current.Services
               .GetService<IPushNotificationService>();
           
           notificationService?.HandleNotification(
               message.GetNotification()?.Title ?? "",
               message.GetNotification()?.Body ?? "",
               message.Data.ToDictionary(x => x.Key, x => x.Value)
           );
       }

       public override void OnNewToken(string token)
       {
           base.OnNewToken(token);
           // Send token to your backend server
           var notificationService = MauiApplication.Current.Services
               .GetService<IPushNotificationService>();
           
           _ = notificationService?.RegisterDeviceAsync(token);
       }
   }
   ```

5. **Update Push Notification Service**

   Modify `Services/PushNotificationService.cs` to initialize Firebase:
   ```csharp
   public async Task InitializeAsync()
   {
       #if ANDROID
       var token = await Firebase.Messaging.FirebaseMessaging.Instance.GetToken();
       await RegisterDeviceAsync(token.ToString());
       #endif
   }
   ```

### Backend Push Notification Endpoint

Your backend should support sending push notifications:

```
POST /notifications/send
Headers: Authorization: Bearer {admin-token}
Body: {
    "deviceTokens": ["token1", "token2"],
    "title": "Notification Title",
    "message": "Notification Body",
    "data": { "key": "value" }
}
```

## Background Service Configuration

### Android Background Service

The `BackgroundSessionService` maintains the user session. To ensure it runs in the background:

1. **Request Battery Optimization Exemption** (Optional)

   Users may need to disable battery optimization for your app to ensure the background service runs consistently.

2. **Foreground Service** (For Long-Running Tasks)

   If you need guaranteed background execution, consider implementing a foreground service:

   Create `Platforms/Android/ForegroundService.cs`:
   ```csharp
   using Android.App;
   using Android.Content;
   using Android.OS;

   [Service]
   public class ForegroundService : Service
   {
       private const int NotificationId = 1000;

       public override IBinder? OnBind(Intent? intent)
       {
           return null;
       }

       public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
       {
           var notification = new NotificationCompat.Builder(this, "app_channel")
               .SetContentTitle("MAUI Study App")
               .SetContentText("App is running in background")
               .SetSmallIcon(Resource.Drawable.icon)
               .SetOngoing(true)
               .Build();

           StartForeground(NotificationId, notification);
           return StartCommandResult.Sticky;
       }
   }
   ```

3. **Create Notification Channel**

   Add to `Platforms/Android/MainActivity.cs`:
   ```csharp
   protected override void OnCreate(Bundle? savedInstanceState)
   {
       base.OnCreate(savedInstanceState);
       
       if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
       {
           var channel = new NotificationChannel(
               "app_channel",
               "App Notifications",
               NotificationImportance.Default
           );
           
           var notificationManager = GetSystemService(NotificationService) as NotificationManager;
           notificationManager?.CreateNotificationChannel(channel);
       }
   }
   ```

## Permission Handling

### Runtime Permissions

For Android 6.0+, request permissions at runtime:

Add permission helper to `Services/PermissionHelper.cs`:
```csharp
public static class PermissionHelper
{
    public static async Task<bool> CheckAndRequestPermission<T>() where T : Permissions.BasePermission, new()
    {
        var status = await Permissions.CheckStatusAsync<T>();
        
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<T>();
        }
        
        return status == PermissionStatus.Granted;
    }
}
```

Update HomePage to request permissions:
```csharp
private async void OnCameraClicked(object sender, EventArgs e)
{
    if (!await PermissionHelper.CheckAndRequestPermission<Permissions.Camera>())
    {
        await DisplayAlertAsync("Permission Denied", "Camera permission is required", "OK");
        return;
    }
    
    // Proceed with camera access
}
```

## Testing

### Testing API Integration

1. Set up a mock backend server
2. Update the `BaseUrl` in `ApiService.cs`
3. Test authentication flow
4. Test data exchange endpoints

### Testing Push Notifications

1. Use Firebase Console to send test notifications
2. Test notification handling when app is:
   - Foreground
   - Background
   - Terminated
3. Verify device token registration with your backend

### Testing Background Service

1. Login to the app
2. Minimize the app
3. Check logs to verify session validation is occurring
4. Wait for session timeout and verify handling

## Security Considerations

1. **Secure Storage**: User credentials are stored using MAUI SecureStorage
2. **HTTPS Only**: Always use HTTPS for API communication
3. **Token Expiration**: Implement token refresh mechanism
4. **Certificate Pinning**: Consider implementing SSL certificate pinning for production
5. **Code Obfuscation**: Use obfuscation tools for production builds

## Troubleshooting

### Common Issues

1. **Push Notifications Not Received**
   - Verify Firebase configuration
   - Check device token registration
   - Ensure Google Play Services is available

2. **Background Service Stops**
   - Check battery optimization settings
   - Consider using foreground service
   - Review Android Doze mode restrictions

3. **API Connection Fails**
   - Verify network permissions
   - Check API URL configuration
   - Review backend CORS settings

## Additional Resources

- [.NET MAUI Documentation](https://docs.microsoft.com/en-us/dotnet/maui/)
- [Firebase Cloud Messaging](https://firebase.google.com/docs/cloud-messaging)
- [Android Background Execution Limits](https://developer.android.com/about/versions/oreo/background)
