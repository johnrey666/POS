//DatabaseSeeder.cs
using Microsoft.EntityFrameworkCore;
using POSSystem.Domain.Entities;
using POSSystem.Domain.Security;
using POSSystem.Infrastructure.Security;

namespace POSSystem.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(PosDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Users.AnyAsync(cancellationToken))
            return;

        var permissions = PermissionCodes.All
            .Select(p => new Permission
            {
                Code = p.Code,
                Name = p.Name,
                Category = p.Category,
                Description = p.Description
            })
            .ToList();

        context.Permissions.AddRange(permissions);
        await context.SaveChangesAsync(cancellationToken);

        var permissionByCode = permissions.ToDictionary(p => p.Code, p => p.Id);

        var cashierRole = new Role
        {
            Name = RoleNames.Cashier,
            Description = "Process sales at the POS terminal",
            IsSystemRole = true
        };

        var supervisorRole = new Role
        {
            Name = RoleNames.CashierSupervisor,
            Description = "Supervise cashiers and approve restricted actions",
            IsSystemRole = true
        };

        var adminRole = new Role
        {
            Name = RoleNames.Admin,
            Description = "Full system access",
            IsSystemRole = true
        };

        context.Roles.AddRange(cashierRole, supervisorRole, adminRole);
        await context.SaveChangesAsync(cancellationToken);

        AssignPermissions(context, cashierRole.Id, permissionByCode,
        [
            PermissionCodes.PosAccess,
            PermissionCodes.SalesCreate,
            PermissionCodes.SalesViewOwn,
            PermissionCodes.SalesHold,
            PermissionCodes.DiscountsApply,
            PermissionCodes.ProductsView,
        ]);

        AssignPermissions(context, supervisorRole.Id, permissionByCode,
        [
            PermissionCodes.PosAccess,
            PermissionCodes.SalesCreate,
            PermissionCodes.SalesViewOwn,
            PermissionCodes.SalesViewBranch,
            PermissionCodes.SalesHold,
            PermissionCodes.SalesVoid,
            PermissionCodes.SalesRefund,
            PermissionCodes.SalesReprint,
            PermissionCodes.DiscountsApply,
            PermissionCodes.DiscountsApprove,
            PermissionCodes.ReportsView,
            PermissionCodes.ProductsView,
        ]);

        AssignPermissions(context, adminRole.Id, permissionByCode,
            PermissionCodes.All.Select(p => p.Code).ToArray());

        await context.SaveChangesAsync(cancellationToken);

        var mainBranch = new Branch
        {
            Name = "Main Branch",
            Code = "BR-001",
            Address = "Manila City Center"
        };

        var secondBranch = new Branch
        {
            Name = "Branch 002",
            Code = "BR-002",
            Address = "Quezon Avenue"
        };

        context.Branches.AddRange(mainBranch, secondBranch);
        await context.SaveChangesAsync(cancellationToken);

        context.Terminals.AddRange(
            new Terminal { Name = "Terminal 01", Code = "T-01", BranchId = mainBranch.Id },
            new Terminal { Name = "Terminal 02", Code = "T-02", BranchId = mainBranch.Id },
            new Terminal { Name = "Terminal 03", Code = "T-03", BranchId = mainBranch.Id },
            new Terminal { Name = "Terminal 01", Code = "T-01", BranchId = secondBranch.Id },
            new Terminal { Name = "Terminal 02", Code = "T-02", BranchId = secondBranch.Id }
        );

        var categories = new[]
        {
            new Category { Name = "Burgers", Description = "Burger meals and combo items" },
            new Category { Name = "Drinks", Description = "Cold and hot beverages" },
            new Category { Name = "Desserts", Description = "Sweet treats and snacks" },
            new Category { Name = "Meals", Description = "Rice and meal bundles" }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync(cancellationToken);

        var burgerCategory = categories[0];
        var drinksCategory = categories[1];
        var dessertCategory = categories[2];
        var mealsCategory = categories[3];

        context.Products.AddRange(
            new Product { Sku = "BRG-001", Barcode = "1001", Name = "Classic Burger", CategoryId = burgerCategory.Id, CostPrice = 55m, SellingPrice = 120m, StockQuantity = 50, IsActive = true },
            new Product { Sku = "BRG-002", Barcode = "1002", Name = "Cheese Burger", CategoryId = burgerCategory.Id, CostPrice = 70m, SellingPrice = 150m, StockQuantity = 40, IsActive = true },
            new Product { Sku = "DRK-001", Barcode = "2001", Name = "Iced Tea", CategoryId = drinksCategory.Id, CostPrice = 20m, SellingPrice = 55m, StockQuantity = 80, IsActive = true },
            new Product { Sku = "DRK-002", Barcode = "2002", Name = "Coke Float", CategoryId = drinksCategory.Id, CostPrice = 25m, SellingPrice = 75m, StockQuantity = 60, IsActive = true },
            new Product { Sku = "DST-001", Barcode = "3001", Name = "Brownie Bite", CategoryId = dessertCategory.Id, CostPrice = 18m, SellingPrice = 45m, StockQuantity = 35, IsActive = true },
            new Product { Sku = "DST-002", Barcode = "3002", Name = "Cookie Sundae", CategoryId = dessertCategory.Id, CostPrice = 28m, SellingPrice = 80m, StockQuantity = 25, IsActive = true },
            new Product { Sku = "ML-001", Barcode = "4001", Name = "Chicken Rice Meal", CategoryId = mealsCategory.Id, CostPrice = 90m, SellingPrice = 180m, StockQuantity = 30, IsActive = true },
            new Product { Sku = "ML-002", Barcode = "4002", Name = "Pork Chop Meal", CategoryId = mealsCategory.Id, CostPrice = 110m, SellingPrice = 210m, StockQuantity = 22, IsActive = true }
        );

        await context.SaveChangesAsync(cancellationToken);

        var cashierUser = CreateUser("cashier", "cashier123", "Demo Cashier", cashierRole.Id);
        cashierUser.BranchId = mainBranch.Id;
        cashierUser.TerminalId = context.Terminals.First(t => t.BranchId == mainBranch.Id && t.Name == "Terminal 01").Id;

        var supervisorUser = CreateUser("cashiersupervisor", "cashiersupervisor123", "Demo Supervisor", supervisorRole.Id);
        supervisorUser.BranchId = mainBranch.Id;
        supervisorUser.TerminalId = context.Terminals.First(t => t.BranchId == mainBranch.Id && t.Name == "Terminal 02").Id;

        var adminUser = CreateUser("admin", "admin123", "Demo Administrator", adminRole.Id);
        adminUser.BranchId = mainBranch.Id;
        adminUser.TerminalId = context.Terminals.First(t => t.BranchId == mainBranch.Id && t.Name == "Terminal 03").Id;

        context.Users.AddRange(cashierUser, supervisorUser, adminUser);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static void AssignPermissions(
        PosDbContext context,
        int roleId,
        Dictionary<string, int> permissionByCode,
        string[] codes)
    {
        foreach (var code in codes)
        {
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionByCode[code]
            });
        }
    }

    private static User CreateUser(string username, string password, string fullName, int roleId)
    {
        var (hash, salt) = PasswordHasher.HashPassword(password);
        return new User
        {
            Username = username,
            PasswordHash = hash,
            PasswordSalt = salt,
            FullName = fullName,
            RoleId = roleId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}
