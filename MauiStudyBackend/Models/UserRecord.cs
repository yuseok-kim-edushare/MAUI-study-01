namespace MauiStudyBackend.Models;

public sealed record UserRecord(Guid Id, string UserName, string? DisplayName, string Role);
