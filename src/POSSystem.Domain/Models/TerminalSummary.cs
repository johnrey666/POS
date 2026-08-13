namespace POSSystem.Domain.Models;

public record TerminalSummary
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? SerialNumber { get; init; }
    public bool IsActive { get; init; }
}