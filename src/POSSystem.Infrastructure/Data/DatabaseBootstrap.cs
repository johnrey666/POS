//DatabaseBootrap.cs
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace POSSystem.Infrastructure.Data;

public static class DatabaseBootstrap
{
    public static string GetDatabasePath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var folder = Path.Combine(appData, "POSSystem");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "pos.db");
    }

    public static PosDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PosDbContext>()
            .UseSqlite($"Data Source={GetDatabasePath()}")
            .Options;

        return new PosDbContext(options);
    }

    public static async Task<(bool Success, string Message)> InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = CreateContext();
            await EnsureSchemaAsync(context, cancellationToken);
            await DatabaseSeeder.SeedAsync(context, cancellationToken);
            return (true, $"SQLite ready — {GetDatabasePath()}");
        }
        catch (Exception ex)
        {
            return (false, $"Database error: {ex.Message}");
        }
    }

    private static async Task EnsureSchemaAsync(PosDbContext context, CancellationToken cancellationToken)
    {
        var shouldReset = !await UsersTableExistsAsync(context, cancellationToken)
            || !await HasRequiredColumnsAsync(context, cancellationToken)
            || !await TableExistsAsync(context, "PromoProducts", cancellationToken)
            || !await TableExistsAsync(context, "ProductBranchPrices", cancellationToken);

        if (shouldReset)
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }

        await context.Database.EnsureCreatedAsync(cancellationToken);
    }

    private static async Task<bool> UsersTableExistsAsync(PosDbContext context, CancellationToken cancellationToken)
    {
        return await TableExistsAsync(context, "Users", cancellationToken);
    }

    private static async Task<bool> TableExistsAsync(PosDbContext context, string tableName, CancellationToken cancellationToken)
    {
        try
        {
            await context.Database.OpenConnectionAsync(cancellationToken);
            await using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = $"SELECT 1 FROM sqlite_master WHERE type='table' AND name='{tableName}' LIMIT 1;";
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result is not null;
        }
        catch (SqliteException)
        {
            return false;
        }
        finally
        {
            await context.Database.CloseConnectionAsync();
        }
    }

    private static async Task<bool> HasRequiredColumnsAsync(PosDbContext context, CancellationToken cancellationToken)
    {
        var requiredColumns = new[] { "RoleId", "BranchId", "TerminalId" };

        try
        {
            await context.Database.OpenConnectionAsync(cancellationToken);
            await using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "PRAGMA table_info('Users');";
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            var columns = new List<string>();
            while (await reader.ReadAsync(cancellationToken))
            {
                var name = reader.GetString(1);
                columns.Add(name);
            }

            return requiredColumns.All(columns.Contains);
        }
        catch (SqliteException)
        {
            return false;
        }
        finally
        {
            await context.Database.CloseConnectionAsync();
        }
    }
}
