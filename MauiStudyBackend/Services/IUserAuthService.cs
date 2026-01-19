using MauiStudyBackend.Models;

namespace MauiStudyBackend.Services;

public interface IUserAuthService
{
    Task<UserRecord?> AuthenticateAsync(string username, string password);
}
