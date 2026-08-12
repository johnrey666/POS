namespace POSSystem.Domain.Entities;

public class Terminal
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Code { get; set; }
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
