using System.Collections.Concurrent;
using MauiStudyBackend.Models;

namespace MauiStudyBackend.Services;

public sealed class InMemoryDeviceRegistry : IDeviceRegistry
{
    private readonly ConcurrentDictionary<string, List<DeviceRegistration>> _devices = new();

    public void Register(string userId, DeviceRegistration registration)
    {
        var list = _devices.GetOrAdd(userId, _ => new List<DeviceRegistration>());
        lock (list)
        {
            list.RemoveAll(d => d.DeviceToken == registration.DeviceToken && d.Platform == registration.Platform);
            list.Add(registration);
        }
    }

    public IReadOnlyList<DeviceRegistration> GetForUser(string userId)
    {
        return _devices.TryGetValue(userId, out var list)
            ? list.AsReadOnly()
            : Array.Empty<DeviceRegistration>();
    }
}
