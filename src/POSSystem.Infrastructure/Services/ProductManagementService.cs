using Microsoft.EntityFrameworkCore;
using POSSystem.Domain.Services;
using POSSystem.Infrastructure.Data;
using POSSystem.Domain.Models;

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
                Sku = p.Sku,
                Barcode = p.Barcode,
                Name = p.Name,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                CostPrice = p.CostPrice,
                SellingPrice = p.SellingPrice,
                StockQuantity = p.StockQuantity,
                IsActive = p.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public async Task CreateProductAsync(ProductManagementItem product, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new InvalidOperationException("Product name is required.");

        await using var context = DatabaseBootstrap.CreateContext();

        var entity = new POSSystem.Domain.Entities.Product
        {
            Sku = product.Sku,
            Barcode = product.Barcode,
            Name = product.Name,
            CategoryId = product.CategoryId,
            CostPrice = product.CostPrice,
            SellingPrice = product.SellingPrice,
            StockQuantity = product.StockQuantity,
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

        entity.Sku = product.Sku;
        entity.Barcode = product.Barcode;
        entity.Name = product.Name;
        entity.CategoryId = product.CategoryId;
        entity.CostPrice = product.CostPrice;
        entity.SellingPrice = product.SellingPrice;
        entity.StockQuantity = product.StockQuantity;
        entity.IsActive = product.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }
    
}
