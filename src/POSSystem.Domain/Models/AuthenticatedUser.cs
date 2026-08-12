namespace POSSystem.Domain.Models;

public sealed record AuthenticatedUser
{
    public required int UserId { get; init; }
    public required string Username { get; init; }
    public required string FullName { get; init; }
    public required int RoleId { get; init; }
    public required string RoleName { get; init; }
    public required IReadOnlySet<string> Permissions { get; init; }
    public int? BranchId { get; init; }
    public string? BranchName { get; init; }
    public int? TerminalId { get; init; }
    public string? TerminalName { get; init; }

    public bool HasPermission(string permissionCode) =>
        Permissions.Contains(permissionCode);
}
