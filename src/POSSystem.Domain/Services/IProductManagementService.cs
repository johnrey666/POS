using POSSystem.Domain.Models;

namespace POSSystem.Domain.Services;

public interface IProductManagementService
{
    Task<IList<ProductManagementItem>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductManagementItem?> GetProductAsync(int productId, CancellationToken cancellationToken = default);
    Task<IList<PromoProductItem>> GetPromoProductsAsync(CancellationToken cancellationToken = default);
    Task CreatePromoProductAsync(int productId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task UpdatePromoProductAsync(int id, int productId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task DeletePromoProductAsync(int id, CancellationToken cancellationToken = default);
    Task<IList<BranchProductListingItem>> GetBranchProductListingsAsync(int? branchId = null, CancellationToken cancellationToken = default);
    Task UpdateBranchProductPriceAsync(int productId, int branchId, decimal price, CancellationToken cancellationToken = default);
    Task AddBranchProductAsync(int productId, int branchId, decimal price, CancellationToken cancellationToken = default);
}