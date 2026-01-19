using MauiStudyBackend.Models;

namespace MauiStudyBackend.Services;

public interface IDeviceRegistry
{
    void Register(string userId, DeviceRegistration registration);
    IReadOnlyList<DeviceRegistration> GetForUser(string userId);
}
