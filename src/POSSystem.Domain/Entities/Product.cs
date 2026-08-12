namespace POSSystem.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public required string Sku { get; set; }
    public required string Barcode { get; set; }
    public required string Name { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
