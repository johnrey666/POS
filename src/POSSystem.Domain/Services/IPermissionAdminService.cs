//POSSystem.Domain/Services/IPermissionAdminService.cs

using POSSystem.Domain.Models;

namespace POSSystem.Domain.Services;

public interface IPermissionAdminService
{
    Task<IList<RoleSummary>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<IList<RolePermissionItem>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default);
    Task SaveRolePermissionsAsync(int roleId, IEnumerable<int> enabledPermissionIds, CancellationToken cancellationToken = default);
}