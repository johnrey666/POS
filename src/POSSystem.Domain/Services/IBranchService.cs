using POSSystem.Domain.Models;

namespace POSSystem.Domain.Services;

public interface IBranchService
{
    Task<IList<BranchSummary>> GetBranchesAsync(CancellationToken cancellationToken = default);
    Task<IList<TerminalSummary>> GetTerminalsAsync(int branchId, CancellationToken cancellationToken = default);
    Task<BranchSummary?> GetBranchAsync(int branchId, CancellationToken cancellationToken = default);
}