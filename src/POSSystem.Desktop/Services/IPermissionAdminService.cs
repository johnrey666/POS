using POSSystem.Domain.Models;

namespace POSSystem.Domain.Services;

public interface IPermissionAdminService
{
    Task<IEnumerable<RoleSummary>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RolePermissionItem>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default);
    // Add other admin methods as needed
}