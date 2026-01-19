using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MauiStudyBackend.Models;
using MauiStudyBackend.Services;

namespace MauiStudyBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class DevicesController : ControllerBase
{
    private readonly IDeviceRegistry _deviceRegistry;

    public DevicesController(IDeviceRegistry deviceRegistry)
    {
        _deviceRegistry = deviceRegistry;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] DeviceRegistration registration)
    {
        if (string.IsNullOrWhiteSpace(registration.DeviceToken) || string.IsNullOrWhiteSpace(registration.Platform))
        {
            return BadRequest(new { message = "DeviceToken and Platform are required." });
        }

        var userId = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        _deviceRegistry.Register(userId, registration);
        return Ok();
    }
}
