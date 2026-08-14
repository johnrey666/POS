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
}