# Implementation Guide

This guide provides detailed instructions for implementing the backend API integration and push notification features using ASP.NET Core.

## Backend API Integration with ASP.NET Core

### Setting Up Your ASP.NET Core Backend API

1. **Create ASP.NET Core Web API Project**
   
   ```bash
   dotnet new webapi -n MauiStudyBackend
   cd MauiStudyBackend
   ```

2. **Update API Base URL in Mobile App**
   
    Pass the base URL at build time using the `ApiBaseUrl` MSBuild property:
    ```bash
    dotnet build -f net10.0-android -c Debug \
      -p:ApiBaseUrl=https://your-backend-api.com/api
    ```

    The app reads this value from assembly metadata at runtime via `ApiService.ResolveBaseUrl()`.

3. **Implement Authentication Endpoint in ASP.NET Core**

   Create `Controllers/AuthController.cs`:
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class AuthController : ControllerBase
   {
       [HttpPost("login")]
       public async Task<IActionResult> Login([FromBody] LoginRequest request)
       {
           // Implement your authentication logic here
           if (ValidateCredentials(request.UserId, request.Password))
           {
               var token = GenerateJwtToken(request.UserId);
               return Ok(new { token });
           }
           return Unauthorized();
       }

       [HttpGet("validate")]
       [Authorize]
       public IActionResult Validate()
       {
           return Ok();
       }
   }

   public class LoginRequest
   {
       public string UserId { get; set; }
       public string Password { get; set; }
   }
   ```

4. **Configure JWT Authentication**

   In `Program.cs`:
   ```csharp
   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidateAudience = true,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ValidIssuer = builder.Configuration["Jwt:Issuer"],
               ValidAudience = builder.Configuration["Jwt:Audience"],
               IssuerSigningKey = new SymmetricSecurityKey(
                   Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
           };
       });
   ```

## Push Notification Setup with ASP.NET Core

### Option 1: Using SignalR for Real-Time Push

1. **Add SignalR to ASP.NET Core Backend**

   ```bash
   dotnet add package Microsoft.AspNetCore.SignalR
   ```

   Create `Hubs/NotificationHub.cs`:
   ```csharp
   public class NotificationHub : Hub
   {
       public async Task SendNotificationToUser(string userId, string title, string message)
       {
           await Clients.User(userId).SendAsync("ReceiveNotification", title, message);
       }
   }
   ```

   Configure in `Program.cs`:
   ```csharp
   builder.Services.AddSignalR();
   app.MapHub<NotificationHub>("/notificationHub");
   ```

2. **Add SignalR Client to Mobile App**

   Add NuGet package:
   ```bash
   dotnet add package Microsoft.AspNetCore.SignalR.Client
   ```

   Update `PushNotificationService.cs`:
   ```csharp
   private HubConnection? _hubConnection;

   public async Task InitializeAsync()
   {
       _hubConnection = new HubConnectionBuilder()
           .WithUrl("https://your-backend-api.com/notificationHub")
           .Build();

       _hubConnection.On<string, string>("ReceiveNotification", 
           (title, message) =>
           {
               HandleNotification(title, message, new Dictionary<string, string>());
           });

       await _hubConnection.StartAsync();
   }
   ```

### Option 2: Using APNs (Apple Push Notification) and FCM with ASP.NET Core

1. **Install Required Packages**

   ```bash
   dotnet add package CorePush
   ```

2. **Create Notification Service in Backend**

   ```csharp
   public interface INotificationService
   {
       Task SendNotificationAsync(string deviceToken, string title, string message, bool isIos);
   }

   public class NotificationService : INotificationService
   {
       public async Task SendNotificationAsync(string deviceToken, string title, string message, bool isIos)
       {
           if (isIos)
           {
               // Send via APNs
               await SendApnsNotification(deviceToken, title, message);
           }
           else
           {
               // Send via FCM
               await SendFcmNotification(deviceToken, title, message);
           }
       }
   }
   ```

3. **Device Token Registration Endpoint**

   ```csharp
   [HttpPost("devices/register")]
   [Authorize]
   public async Task<IActionResult> RegisterDevice([FromBody] DeviceRegistration registration)
   {
       // Store device token in database associated with user
       await _deviceRepository.RegisterDeviceAsync(
           User.Identity.Name, 
           registration.DeviceToken, 
           registration.Platform);
       return Ok();
   }
   ```

## Platform-Specific Configurations

### Android (API 30-36 with 16KB Page Support)

The project is configured for:
- **Minimum SDK**: API 30 (Android 11)
- **Target SDK**: API 36 (Android 15)
- **16KB Page Size**: Enabled via `AndroidPageSize` property

Additional considerations:
- Test on devices with 16KB page granularity
- Use R8 for code optimization
- Ensure all native libraries support 16KB pages

### iOS (iOS 17.0+)

The project is configured for:
- **Minimum iOS Version**: 17.0
- **Target SDK**: iOS 26 SDK (Xcode 26)

Required permissions configured in Info.plist:
- Camera usage
- Photo library access
- Location services
- Background modes for notifications

## Using the API Service

The `IApiService` interface provides methods for:
- `AuthenticateAsync()` - User authentication with backend
- `SendDataAsync()` - Send data to backend
- `GetDataAsync<T>()` - Retrieve data from backend
- `ValidateSessionAsync()` - Check session validity

## Background Service Configuration

### Android Background Service (API 30-35)

The `BackgroundSessionService` maintains the user session. For Android 11+ (API 30+):

1. **Foreground Service Implementation**

   Create `Platforms/Android/SessionForegroundService.cs`:
   ```csharp
   using Android.App;
   using Android.Content;
   using Android.OS;
   using AndroidX.Core.App;

   [Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeDataSync)]
   public class SessionForegroundService : Service
   {
       private const int NotificationId = 1000;
       private const string ChannelId = "session_channel";

       public override IBinder? OnBind(Intent? intent)
       {
           return null;
       }

       public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
       {
           CreateNotificationChannel();
           
           var notification = new NotificationCompat.Builder(this, ChannelId)
               .SetContentTitle("MAUI Study App")
               .SetContentText("Maintaining session in background")
               .SetSmallIcon(Resource.Drawable.icon)
               .SetOngoing(true)
               .SetPriority(NotificationCompat.PriorityLow)
               .Build();

           StartForeground(NotificationId, notification);
           return StartCommandResult.Sticky;
       }

       private void CreateNotificationChannel()
       {
           if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
           {
               var channel = new NotificationChannel(
                   ChannelId,
                   "Session Notifications",
                   NotificationImportance.Low
               );
               
               var notificationManager = GetSystemService(NotificationService) as NotificationManager;
               notificationManager?.CreateNotificationChannel(channel);
           }
       }
   }
   ```

2. **Request Battery Optimization Exemption** (Optional)

   Users may need to disable battery optimization for consistent background execution.

### iOS Background Service

For iOS 17.0+, configure background fetch:

Add to `Platforms/iOS/AppDelegate.cs`:
```csharp
public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
{
    UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(
        UIApplication.BackgroundFetchIntervalMinimum);
    return base.FinishedLaunching(application, launchOptions);
}
```

## Permission Handling

### Runtime Permissions (Cross-Platform)

Create `Services/PermissionHelper.cs`:
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
        await this.DisplayAlertAsync("Permission Denied", "Camera permission is required", "OK");
        return;
    }
    
    // Proceed with camera access
}
```

## Testing

### Testing API Integration with ASP.NET Core

1. Run your ASP.NET Core backend locally or deploy to Azure
2. Update the `BaseUrl` in `ApiService.cs` to point to your backend
3. Test authentication flow with JWT tokens
4. Test data exchange endpoints
5. Verify session validation works correctly

### Testing Push Notifications

**With SignalR:**
1. Connect to SignalR hub from mobile app
2. Send notifications from backend
3. Verify real-time delivery

**With APNs/FCM:**
1. Send test notifications from your backend
2. Test notification handling when app is:
   - Foreground
   - Background
   - Terminated
3. Verify device token registration with backend

### Testing on Target Platforms

**Android (API 30-36 with 16KB pages):**
- Test on Android 11, 14, and 15 devices
- Verify 16KB page size compatibility
- Test foreground service behavior
- Check notification channels

**iOS (17.0+):**
- Test on iOS 17+ devices
- Verify background fetch works
- Check all permissions are properly requested
- Test push notification delivery

## Security Considerations

1. **Secure Storage**: User credentials stored using MAUI SecureStorage
2. **HTTPS Only**: Always use HTTPS for API communication
3. **JWT Token Security**: Implement token refresh mechanism
4. **Certificate Pinning**: Consider SSL certificate pinning for production
5. **Code Obfuscation**: Use obfuscation tools for production builds
6. **API Rate Limiting**: Implement rate limiting on backend
7. **Input Validation**: Validate all inputs on both client and server

## Troubleshooting

### Common Issues

1. **Push Notifications Not Received**
   - Verify SignalR connection is established
   - Check device token is registered with backend
   - Ensure background modes are configured

2. **Background Service Stops (Android)**
   - Check battery optimization settings
   - Verify foreground service is properly configured
   - Review Android Doze mode restrictions
   - Ensure FOREGROUND_SERVICE permission is granted

3. **API Connection Fails**
   - Verify network permissions
   - Check API URL configuration
   - Review backend CORS settings
   - Ensure JWT token is valid

4. **16KB Page Size Issues (Android)**
   - Update all native libraries to support 16KB pages
   - Test on devices with different page sizes
   - Check R8/ProGuard configuration

## Additional Resources

- [.NET MAUI Documentation](https://docs.microsoft.com/en-us/dotnet/maui/)
- [ASP.NET Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [SignalR Documentation](https://docs.microsoft.com/en-us/aspnet/core/signalr/)
- [Android 15 Compatibility](https://developer.android.com/about/versions/15/behavior-changes-15)
- [iOS Background Execution](https://developer.apple.com/documentation/uikit/app_and_environment/scenes/preparing_your_ui_to_run_in_the_background)
