namespace POSSystem.Domain.Services;

public interface IAuthorizationService
{
    bool HasPermission(string permissionCode);
    void EnsurePermission(string permissionCode);
}
