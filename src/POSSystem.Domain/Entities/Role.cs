namespace POSSystem.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }

    public ICollection<User> Users { get; set; } = [];
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
