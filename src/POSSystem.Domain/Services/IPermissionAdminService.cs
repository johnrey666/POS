namespace POSSystem.Domain.Services;

public interface IPermissionAdminService
{
    Task<IReadOnlyList<RoleSummary>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RolePermissionItem>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default);
    Task SaveRolePermissionsAsync(int roleId, IReadOnlyCollection<int> enabledPermissionIds, CancellationToken cancellationToken = default);
}

public sealed class RoleSummary
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}

public sealed class RolePermissionItem
{
    public required int PermissionId { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Category { get; init; }
    public required bool IsEnabled { get; init; }
}
