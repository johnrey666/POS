namespace POSSystem.Domain.Models;

public record CategorySummary
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}