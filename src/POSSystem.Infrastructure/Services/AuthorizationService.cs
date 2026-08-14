using POSSystem.Domain.Models;
using POSSystem.Domain.Services;

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
        return user?.Permissions.Contains(permissionCode) ?? false;
    }

    public bool IsInRole(string roleName)
    {
        return _authService.CurrentUser?.RoleName == roleName;
    }
}