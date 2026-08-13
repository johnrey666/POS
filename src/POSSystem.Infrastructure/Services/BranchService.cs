using Microsoft.EntityFrameworkCore;
using POSSystem.Domain.Services;
using POSSystem.Infrastructure.Data;
using POSSystem.Domain.Models;

namespace POSSystem.Infrastructure.Services;

public sealed class BranchService : IBranchService
{
    public async Task<IList<BranchSummary>> GetBranchesAsync(CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        return await context.Branches
            .Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .Select(b => new BranchSummary
            {
                Id = b.Id,
                Name = b.Name,
                Code = b.Code,
                Address = b.Address
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TerminalSummary>> GetTerminalsAsync(int branchId, CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        return await context.Terminals
            .Where(t => t.BranchId == branchId && t.IsActive)
            .OrderBy(t => t.Name)
            .Select(t => new TerminalSummary
            {
                Id = t.Id,
                Name = t.Name,
                Code = t.Code,
                BranchId = t.BranchId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BranchSummary>> GetUserBranchesAsync(int userId, CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        return await context.Users
            .Where(u => u.Id == userId && u.BranchId.HasValue)
            .Select(u => new { u.BranchId })
            .Join(
                context.Branches,
                u => u.BranchId,
                b => b.Id,
                (u, b) => new BranchSummary
                {
                    Id = b.Id,
                    Name = b.Name,
                    Code = b.Code,
                    Address = b.Address
                })
            .ToListAsync(cancellationToken);
    }
    
}
