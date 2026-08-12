using Microsoft.EntityFrameworkCore;
using POSSystem.Domain.Services;
using POSSystem.Infrastructure.Data;

namespace POSSystem.Infrastructure.Services;

public sealed class PermissionAdminService : IPermissionAdminService
{
    private readonly IAuthService _authService;
    private readonly IAuthorizationService _authorizationService;

    public PermissionAdminService(IAuthService authService, IAuthorizationService authorizationService)
    {
        _authService = authService;
        _authorizationService = authorizationService;
    }

    public async Task<IReadOnlyList<RoleSummary>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        _authorizationService.EnsurePermission(Domain.Security.PermissionCodes.PermissionsManage);

        await using var context = DatabaseBootstrap.CreateContext();
        return await context.Roles
            .OrderBy(r => r.Name)
            .Select(r => new RoleSummary
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RolePermissionItem>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default)
    {
        _authorizationService.EnsurePermission(Domain.Security.PermissionCodes.PermissionsManage);

        await using var context = DatabaseBootstrap.CreateContext();

        var enabledIds = await context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToHashSetAsync(cancellationToken);

        return await context.Permissions
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Name)
            .Select(p => new RolePermissionItem
            {
                PermissionId = p.Id,
                Code = p.Code,
                Name = p.Name,
                Category = p.Category,
                IsEnabled = enabledIds.Contains(p.Id)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task SaveRolePermissionsAsync(int roleId, IReadOnlyCollection<int> enabledPermissionIds, CancellationToken cancellationToken = default)
    {
        _authorizationService.EnsurePermission(Domain.Security.PermissionCodes.PermissionsManage);

        await using var context = DatabaseBootstrap.CreateContext();

        var role = await context.Roles.FindAsync([roleId], cancellationToken)
            ?? throw new InvalidOperationException("Role not found.");

        var existing = await context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken);

        context.RolePermissions.RemoveRange(existing);

        foreach (var permissionId in enabledPermissionIds.Distinct())
        {
            context.RolePermissions.Add(new Domain.Entities.RolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId
            });
        }

        await context.SaveChangesAsync(cancellationToken);
        await _authService.RefreshCurrentUserPermissionsAsync(cancellationToken);
    }
}
