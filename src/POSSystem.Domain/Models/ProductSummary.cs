namespace POSSystem.Domain.Models;

public record ProductSummary
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public decimal SellingPrice { get; init; }  // matches usage in PosViewModel
    public int CategoryId { get; init; }
}