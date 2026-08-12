using POSSystem.Domain.Models;

namespace POSSystem.Domain.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
    Task RefreshCurrentUserPermissionsAsync(CancellationToken cancellationToken = default);
    void Logout();
    AuthenticatedUser? CurrentUser { get; }
}

public sealed class AuthResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public AuthenticatedUser? User { get; init; }

    public static AuthResult Succeeded(AuthenticatedUser user) =>
        new() { Success = true, User = user };

    public static AuthResult Failed(string message) =>
        new() { Success = false, ErrorMessage = message };
}
