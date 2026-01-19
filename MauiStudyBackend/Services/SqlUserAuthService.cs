using System.Data;
using BCrypt.Net;
using Microsoft.Data.SqlClient;
using MauiStudyBackend.Models;

namespace MauiStudyBackend.Services;

public sealed class SqlUserAuthService : IUserAuthService
{
    private readonly IConfiguration _configuration;

    public SqlUserAuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<UserRecord?> AuthenticateAsync(string username, string password)
    {
        var connectionString = _configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:Default is not configured.");
        }

        var mode = _configuration["Auth:PasswordMode"]?.ToLowerInvariant() ?? "bcrypt";

        return mode switch
        {
            "pwdcompare" => await AuthenticateWithPwdCompareAsync(connectionString, username, password),
            _ => await AuthenticateWithBcryptAsync(connectionString, username, password)
        };
    }

    private static async Task<UserRecord?> AuthenticateWithBcryptAsync(
        string connectionString,
        string username,
        string password)
    {
        const string sql = @"
SELECT id, username, passwordhash, displayname, Role
FROM dbo.tb_web_user
WHERE username = @username;";

        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar, 256) { Value = username });

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

        if (!await reader.ReadAsync())
        {
            return null;
        }

        var passwordHash = reader.GetString(reader.GetOrdinal("passwordhash"));
        if (!BCrypt.Net.BCrypt.Verify(password, passwordHash))
        {
            return null;
        }

        var id = reader.GetGuid(reader.GetOrdinal("id"));
        var userName = reader.GetString(reader.GetOrdinal("username"));
        var displayNameOrdinal = reader.GetOrdinal("displayname");
        var displayName = reader.IsDBNull(displayNameOrdinal) ? null : reader.GetString(displayNameOrdinal);
        var role = reader.GetString(reader.GetOrdinal("Role"));

        return new UserRecord(id, userName, displayName, role);
    }

    private static async Task<UserRecord?> AuthenticateWithPwdCompareAsync(
        string connectionString,
        string username,
        string password)
    {
        const string sql = @"
SELECT id, username, displayname, Role
FROM dbo.tb_web_user
WHERE username = @username
  AND PWDCOMPARE(@password, passwordhash) = 1;";

        await using var connection = new SqlConnection(connectionString);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar, 256) { Value = username });
        command.Parameters.Add(new SqlParameter("@password", SqlDbType.VarChar, 256) { Value = password });

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

        if (!await reader.ReadAsync())
        {
            return null;
        }

        var id = reader.GetGuid(reader.GetOrdinal("id"));
        var userName = reader.GetString(reader.GetOrdinal("username"));
        var displayNameOrdinal = reader.GetOrdinal("displayname");
        var displayName = reader.IsDBNull(displayNameOrdinal) ? null : reader.GetString(displayNameOrdinal);
        var role = reader.GetString(reader.GetOrdinal("Role"));

        return new UserRecord(id, userName, displayName, role);
    }
}
