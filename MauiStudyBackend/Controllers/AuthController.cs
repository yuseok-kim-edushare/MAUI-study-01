using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MauiStudyBackend.Models;
using MauiStudyBackend.Services;

namespace MauiStudyBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IUserAuthService _userAuthService;

    public AuthController(ITokenService tokenService, IUserAuthService userAuthService)
    {
        _tokenService = tokenService;
        _userAuthService = userAuthService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "UserId and Password are required." });
        }

        var user = await _userAuthService.AuthenticateAsync(request.UserId, request.Password);
        if (user is null)
        {
            return Unauthorized();
        }

        var token = _tokenService.CreateToken(user.UserName, user.Role);
        return Ok(new LoginResponse(token, user.UserName));
    }

    [Authorize]
    [HttpGet("validate")]
    public IActionResult Validate()
    {
        return Ok(new { userId = User.Identity?.Name });
    }
}
