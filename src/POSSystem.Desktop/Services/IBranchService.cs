using POSSystem.Domain.Models;

namespace POSSystem.Domain.Services;

public interface IBranchService
{
    Task<IEnumerable<BranchSummary>> GetBranchesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TerminalSummary>> GetTerminalsAsync(int branchId, CancellationToken cancellationToken = default);
    Task<BranchSummary?> GetBranchAsync(int branchId, CancellationToken cancellationToken = default);
}