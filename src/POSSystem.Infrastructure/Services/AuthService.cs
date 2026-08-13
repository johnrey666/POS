//Services/AuthService.cs
using Microsoft.EntityFrameworkCore;
using POSSystem.Domain.Entities;
using POSSystem.Domain.Models;
using POSSystem.Domain.Security;
using POSSystem.Domain.Services;
using POSSystem.Infrastructure.Data;
using POSSystem.Infrastructure.Security;

namespace POSSystem.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private AuthenticatedUser? _currentUser;

    public AuthenticatedUser? CurrentUser => _currentUser;

    public async Task<AuthResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            return AuthResult.Failed("Username and password are required.");

        await using var context = DatabaseBootstrap.CreateContext();

        var user = await context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Username == username.Trim(), cancellationToken);

        if (user is null || !user.IsActive)
            return AuthResult.Failed("Invalid username or password.");

        if (!PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            return AuthResult.Failed("Invalid username or password.");

        var permissions = user.Role.RolePermissions
            .Select(rp => rp.Permission.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var branchId = user.BranchId ?? 1;
        var branchName = user.Branch?.Name ?? "Main Branch";
        var terminalId = user.TerminalId ?? 1;
        var terminalName = user.Terminal?.Name ?? "Terminal 01";

        _currentUser = new AuthenticatedUser
        {
            UserId = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            RoleId = user.RoleId,
            RoleName = user.Role.Name,
            Permissions = permissions,
            BranchId = branchId,
            BranchName = branchName,
            TerminalId = terminalId,
            TerminalName = terminalName
        };

        return AuthResult.Succeeded(_currentUser);
    }

    public async Task RefreshCurrentUserPermissionsAsync(CancellationToken cancellationToken = default)
    {
        if (_currentUser is null)
            return;

        await using var context = DatabaseBootstrap.CreateContext();

        var user = await context.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken);

        if (user is null)
        {
            Logout();
            return;
        }

        var permissions = user.Role.RolePermissions
            .Select(rp => rp.Permission.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        _currentUser = _currentUser with { Permissions = permissions };
    }

    public void Logout() => _currentUser = null;
}
