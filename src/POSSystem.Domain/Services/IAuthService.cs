using POSSystem.Domain.Models;

namespace POSSystem.Domain.Services;

public interface IAuthService
{
    AuthenticatedUser? CurrentUser { get; }
    Task<AuthResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
    Task RefreshCurrentUserPermissionsAsync(CancellationToken cancellationToken = default);
    void Logout();
}