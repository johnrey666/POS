namespace POSSystem.Domain.Models;

public record BranchSummary
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Code { get; init; }
    public bool IsActive { get; init; }
}