using POSSystem.Domain.Services;
using POSSystem.Domain.Models;

namespace POSSystem.Infrastructure.Services;

public sealed class AuthorizationService : IAuthorizationService
{
    private readonly IAuthService _authService;

    public AuthorizationService(IAuthService authService)
    {
        _authService = authService;
    }

    public bool HasPermission(string permissionCode)
    {
        var user = _authService.CurrentUser;
        return user?.HasPermission(permissionCode) ?? false;
    }

    public void EnsurePermission(string permissionCode)
    {
        if (!HasPermission(permissionCode))
            throw new UnauthorizedAccessException($"Permission denied: {permissionCode}");
    }
    public bool IsInRole(string roleName)
{
    // Simple implementation – adjust if you have a different way to get the current user
    return AppServices.Auth.CurrentUser?.RoleName == roleName;
}
}
