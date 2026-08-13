namespace POSSystem.Domain.Models;

public record AuthenticatedUser
{
    public int UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public int RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public HashSet<string> Permissions { get; init; } = new();
    public int BranchId { get; init; }
    public string BranchName { get; init; } = string.Empty;
    public int TerminalId { get; init; }
    public string TerminalName { get; init; } = string.Empty;
}