using Microsoft.EntityFrameworkCore;
using POSSystem.Domain.Models;
using POSSystem.Domain.Services;
using POSSystem.Infrastructure.Data;

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
                IsActive = b.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<TerminalSummary>> GetTerminalsAsync(int branchId, CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        return await context.Terminals
            .Where(t => t.BranchId == branchId && t.IsActive)
            .OrderBy(t => t.Name)
            .Select(t => new TerminalSummary
            {
                Id = t.Id,
                Name = t.Name,
                SerialNumber = t.Code,           // Map entity's Code to DTO's SerialNumber
                IsActive = t.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<BranchSummary?> GetBranchAsync(int branchId, CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        return await context.Branches
            .Where(b => b.Id == branchId)
            .Select(b => new BranchSummary
            {
                Id = b.Id,
                Name = b.Name,
                Code = b.Code,
                IsActive = b.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}