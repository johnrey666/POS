namespace POSSystem.Domain.Models;

public record BranchProductListingItem
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? Sku { get; init; }
    public int BranchId { get; init; }
    public string BranchName { get; init; } = string.Empty;
    public decimal BranchPrice { get; init; }
    public decimal MasterPrice { get; init; }
    public bool IsActive { get; init; }
}
