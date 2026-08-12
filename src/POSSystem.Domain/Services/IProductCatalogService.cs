namespace POSSystem.Domain.Services;

public interface IProductCatalogService
{
    Task<IReadOnlyList<CategorySummary>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductSummary>> GetProductsAsync(int? categoryId = null, string? search = null, CancellationToken cancellationToken = default);
}

public sealed class CategorySummary
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}

public sealed class ProductSummary
{
    public required int Id { get; init; }
    public required string Sku { get; init; }
    public required string Barcode { get; init; }
    public required string Name { get; init; }
    public required int CategoryId { get; init; }
    public required string CategoryName { get; init; }
    public required decimal CostPrice { get; init; }
    public required decimal SellingPrice { get; init; }
    public required int StockQuantity { get; init; }
    public required bool IsActive { get; init; }
    public string? ImagePath { get; init; }
}
