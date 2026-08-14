using POSSystem.Domain.Models;

namespace POSSystem.Domain.Services;

public interface IProductCatalogService
{
    Task<IList<CategorySummary>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IList<ProductSummary>> GetProductsAsync(int? categoryId = null, string? search = null, CancellationToken cancellationToken = default);
}