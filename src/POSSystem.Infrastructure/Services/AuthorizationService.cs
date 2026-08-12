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
        return user?.HasPermission(permissionCode) ?? false;
    }

    public void EnsurePermission(string permissionCode)
    {
        if (!HasPermission(permissionCode))
            throw new UnauthorizedAccessException($"Permission denied: {permissionCode}");
    }
}
