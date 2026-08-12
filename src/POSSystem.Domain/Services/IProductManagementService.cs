namespace POSSystem.Domain.Services;

public interface IProductManagementService
{
    Task<IReadOnlyList<ProductManagementItem>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task CreateProductAsync(ProductManagementItem product, CancellationToken cancellationToken = default);
    Task UpdateProductAsync(ProductManagementItem product, CancellationToken cancellationToken = default);
}

public sealed class ProductManagementItem
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
}
