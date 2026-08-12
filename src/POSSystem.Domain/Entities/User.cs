namespace POSSystem.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public required string PasswordSalt { get; set; }
    public required string FullName { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }
    public int? TerminalId { get; set; }
    public Terminal? Terminal { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
