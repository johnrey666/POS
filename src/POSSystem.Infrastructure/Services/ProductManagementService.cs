using Microsoft.EntityFrameworkCore;
using POSSystem.Domain.Models;
using POSSystem.Domain.Services;
using POSSystem.Infrastructure.Data;

namespace POSSystem.Infrastructure.Services;

public sealed class ProductManagementService : IProductManagementService
{
    public async Task<IList<ProductManagementItem>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        return await context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .Select(p => new ProductManagementItem
            {
                Id = p.Id,
                Name = p.Name,
                Barcode = p.Barcode,
                Sku = p.Sku,
                SellingPrice = p.SellingPrice,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                IsActive = p.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductManagementItem?> GetProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        return await context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.Id == productId)
            .Select(p => new ProductManagementItem
            {
                Id = p.Id,
                Name = p.Name,
                Barcode = p.Barcode,
                Sku = p.Sku,
                SellingPrice = p.SellingPrice,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                IsActive = p.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task CreateProductAsync(ProductManagementItem product, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new InvalidOperationException("Product name is required.");

        await using var context = DatabaseBootstrap.CreateContext();

        var entity = new POSSystem.Domain.Entities.Product
        {
            Name = product.Name,
            Barcode = product.Barcode ?? string.Empty,
            Sku = product.Sku ?? string.Empty,
            SellingPrice = product.SellingPrice,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId,
            IsActive = product.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Products.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateProductAsync(ProductManagementItem product, CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        var entity = await context.Products.FirstOrDefaultAsync(p => p.Id == product.Id, cancellationToken)
            ?? throw new InvalidOperationException("Product not found.");

        entity.Name = product.Name;
        entity.Barcode = product.Barcode ?? string.Empty;
        entity.Sku = product.Sku ?? string.Empty;
        entity.SellingPrice = product.SellingPrice;
        entity.StockQuantity = product.StockQuantity;
        entity.CategoryId = product.CategoryId;
        entity.IsActive = product.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<PromoProductItem>> GetPromoProductsAsync(CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        return await context.PromoProducts
            .AsNoTracking()
            .Include(pp => pp.Product)
            .OrderByDescending(pp => pp.StartDate)
            .Select(pp => new PromoProductItem
            {
                Id = pp.Id,
                ProductId = pp.ProductId,
                ProductName = pp.Product.Name,
                Sku = pp.Product.Sku,
                SellingPrice = pp.Product.SellingPrice,
                StartDate = pp.StartDate,
                EndDate = pp.EndDate,
                IsActive = pp.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task CreatePromoProductAsync(int productId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        if (endDate < startDate)
            throw new InvalidOperationException("End date must be after start date.");

        await using var context = DatabaseBootstrap.CreateContext();

        var entity = new POSSystem.Domain.Entities.PromoProduct
        {
            ProductId = productId,
            StartDate = startDate,
            EndDate = endDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.PromoProducts.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePromoProductAsync(int id, int productId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        if (endDate < startDate)
            throw new InvalidOperationException("End date must be after start date.");

        await using var context = DatabaseBootstrap.CreateContext();

        var entity = await context.PromoProducts.FirstOrDefaultAsync(pp => pp.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Promotional product not found.");

        entity.ProductId = productId;
        entity.StartDate = startDate;
        entity.EndDate = endDate;
        entity.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeletePromoProductAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        var entity = await context.PromoProducts.FirstOrDefaultAsync(pp => pp.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Promotional product not found.");

        context.PromoProducts.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<BranchProductListingItem>> GetBranchProductListingsAsync(int? branchId = null, CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        var query = context.ProductBranchPrices
            .AsNoTracking()
            .Include(pbp => pbp.Product)
            .Include(pbp => pbp.Branch)
            .OrderBy(pbp => pbp.Branch.Name)
            .ThenBy(pbp => pbp.Product.Name)
            .AsQueryable();

        if (branchId.HasValue)
            query = query.Where(pbp => pbp.BranchId == branchId.Value);

        return await query.Select(pbp => new BranchProductListingItem
        {
            ProductId = pbp.ProductId,
            ProductName = pbp.Product.Name,
            Sku = pbp.Product.Sku,
            BranchId = pbp.BranchId,
            BranchName = pbp.Branch.Name,
            BranchPrice = pbp.Price,
            MasterPrice = pbp.Product.SellingPrice,
            IsActive = pbp.IsActive
        }).ToListAsync(cancellationToken);
    }

    public async Task UpdateBranchProductPriceAsync(int productId, int branchId, decimal price, CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        var entity = await context.ProductBranchPrices
            .FirstOrDefaultAsync(pbp => pbp.ProductId == productId && pbp.BranchId == branchId, cancellationToken)
            ?? throw new InvalidOperationException("Branch product listing not found.");

        entity.Price = price;
        entity.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddBranchProductAsync(int productId, int branchId, decimal price, CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        var exists = await context.ProductBranchPrices
            .AnyAsync(pbp => pbp.ProductId == productId && pbp.BranchId == branchId, cancellationToken);

        if (exists)
            throw new InvalidOperationException("This product is already listed for this branch.");

        var entity = new POSSystem.Domain.Entities.ProductBranchPrice
        {
            ProductId = productId,
            BranchId = branchId,
            Price = price,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.ProductBranchPrices.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
    }
}