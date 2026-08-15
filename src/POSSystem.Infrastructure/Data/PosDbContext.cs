using Microsoft.EntityFrameworkCore;
using POSSystem.Domain.Entities;
using POSSystem.Domain.Security;
using POSSystem.Infrastructure.Security;

namespace POSSystem.Infrastructure.Data;

public class PosDbContext : DbContext
{
    public PosDbContext(DbContextOptions<PosDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Terminal> Terminals => Set<Terminal>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductBranchPrice> ProductBranchPrices => Set<ProductBranchPrice>();
    public DbSet<PromoProduct> PromoProducts => Set<PromoProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).HasMaxLength(50);
            entity.Property(u => u.FullName).HasMaxLength(100);
            entity.Property(u => u.PasswordHash).HasMaxLength(256);
            entity.Property(u => u.PasswordSalt).HasMaxLength(128);

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(u => u.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(u => u.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(u => u.Terminal)
                .WithMany()
                .HasForeignKey(u => u.TerminalId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(r => r.Name).IsUnique();
            entity.Property(r => r.Name).HasMaxLength(50);
            entity.Property(r => r.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasIndex(p => p.Code).IsUnique();
            entity.Property(p => p.Code).HasMaxLength(80);
            entity.Property(p => p.Name).HasMaxLength(100);
            entity.Property(p => p.Category).HasMaxLength(50);
            entity.Property(p => p.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasIndex(b => b.Name).IsUnique();
            entity.Property(b => b.Name).HasMaxLength(80);
            entity.Property(b => b.Code).HasMaxLength(20);
            entity.Property(b => b.Address).HasMaxLength(200);
        });

        modelBuilder.Entity<Terminal>(entity =>
        {
            entity.HasIndex(t => new { t.BranchId, t.Name }).IsUnique();
            entity.Property(t => t.Name).HasMaxLength(80);
            entity.Property(t => t.Code).HasMaxLength(20);

            entity.HasOne(t => t.Branch)
                .WithMany(b => b.Terminals)
                .HasForeignKey(t => t.BranchId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(c => c.Name).IsUnique();
            entity.Property(c => c.Name).HasMaxLength(80);
            entity.Property(c => c.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(p => p.Sku).IsUnique();
            entity.HasIndex(p => p.Barcode).IsUnique();
            entity.Property(p => p.Sku).HasMaxLength(50);
            entity.Property(p => p.Barcode).HasMaxLength(50);
            entity.Property(p => p.Name).HasMaxLength(120);
            entity.Property(p => p.ImagePath).HasMaxLength(250);
            entity.Property(p => p.CostPrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.SellingPrice).HasColumnType("decimal(18,2)");

            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductBranchPrice>(entity =>
        {
            entity.HasKey(pbp => new { pbp.ProductId, pbp.BranchId });

            entity.Property(pbp => pbp.Price).HasColumnType("decimal(18,2)");

            entity.HasOne(pbp => pbp.Product)
                .WithMany()
                .HasForeignKey(pbp => pbp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pbp => pbp.Branch)
                .WithMany()
                .HasForeignKey(pbp => pbp.BranchId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PromoProduct>(entity =>
        {
            entity.HasIndex(pp => new { pp.ProductId, pp.StartDate, pp.EndDate });

            entity.HasOne(pp => pp.Product)
                .WithMany()
                .HasForeignKey(pp => pp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            entity.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
