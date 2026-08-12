namespace POSSystem.Domain.Services;

public interface IBranchService
{
    Task<IReadOnlyList<BranchSummary>> GetBranchesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TerminalSummary>> GetTerminalsAsync(int branchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BranchSummary>> GetUserBranchesAsync(int userId, CancellationToken cancellationToken = default);
}

public sealed class BranchSummary
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? Code { get; init; }
    public string? Address { get; init; }
}

public sealed class TerminalSummary
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? Code { get; init; }
    public required int BranchId { get; init; }
}
