namespace POSSystem.Domain.Entities;

public class Branch
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Code { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<User> Users { get; set; } = [];
    public ICollection<Terminal> Terminals { get; set; } = [];
}
