namespace POSSystem.Domain.Models;

public record RolePermissionItem
{
    public int PermissionId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public bool IsEnabled { get; init; }
}