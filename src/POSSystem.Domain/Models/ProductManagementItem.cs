namespace POSSystem.Domain.Models;

public record ProductManagementItem
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public string? Sku { get; init; }          // Added
    public decimal SellingPrice { get; init; }
    public int StockQuantity { get; init; }
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}