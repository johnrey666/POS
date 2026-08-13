namespace POSSystem.Domain.Services;

public interface IAuthorizationService
{
    bool HasPermission(string permissionCode);
    bool IsInRole(string roleName);
    // Add any other methods you need
}