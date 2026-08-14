using Microsoft.EntityFrameworkCore;
using POSSystem.Domain.Models;
using POSSystem.Domain.Services;
using POSSystem.Infrastructure.Data;

namespace POSSystem.Infrastructure.Services;

public sealed class ProductCatalogService : IProductCatalogService
{
    public async Task<IList<CategorySummary>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        return await context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new CategorySummary
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<ProductSummary>> GetProductsAsync(int? categoryId = null, string? search = null, CancellationToken cancellationToken = default)
    {
        await using var context = DatabaseBootstrap.CreateContext();

        var query = context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                p.Name.Contains(term) ||
                p.Barcode.Contains(term) ||
                p.Sku.Contains(term));
        }

        return await query
            .OrderBy(p => p.Name)
            .Select(p => new ProductSummary
            {
                Id = p.Id,
                Name = p.Name,
                Barcode = p.Barcode,
                SellingPrice = p.SellingPrice,
                CategoryId = p.CategoryId
            })
            .ToListAsync(cancellationToken);
    }
}