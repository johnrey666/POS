using POSSystem.Domain.Models;

namespace POSSystem.Domain.Services;

public interface IProductManagementService
{
    Task<IList<ProductManagementItem>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductManagementItem?> GetProductAsync(int productId, CancellationToken cancellationToken = default);
}