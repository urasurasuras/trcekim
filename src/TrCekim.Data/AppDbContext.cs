using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrCekim.Core;

namespace TrCekim.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<School> Schools => Set<School>();
    public DbSet<Gallery> Galleries => Set<Gallery>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartLine> CartLines => Set<CartLine>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<School>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200);
            e.Property(x => x.City).HasMaxLength(100);
        });

        b.Entity<Gallery>(e =>
        {
            e.Property(x => x.Season).HasMaxLength(20);
            e.Property(x => x.Title).HasMaxLength(200);
            e.Property(x => x.GroupName).HasMaxLength(100);
            e.Property(x => x.AccessCode).HasMaxLength(32);
            e.HasIndex(x => x.AccessCode).IsUnique();
            e.HasOne(x => x.School).WithMany(x => x.Galleries)
                .HasForeignKey(x => x.SchoolId).OnDelete(DeleteBehavior.Restrict);
            e.Ignore(x => x.DisplayName);
        });

        b.Entity<Photo>(e =>
        {
            e.Property(x => x.Number).HasMaxLength(64);
            e.Property(x => x.OriginalFileName).HasMaxLength(260);
            e.HasIndex(x => new { x.GalleryId, x.SortOrder });
            e.HasOne(x => x.Gallery).WithMany(x => x.Photos)
                .HasForeignKey(x => x.GalleryId).OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Product>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(100);
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Price).HasPrecision(10, 2);
        });

        b.Entity<CartLine>(e =>
        {
            e.HasOne<Cart>().WithMany(x => x.Lines).HasForeignKey(x => x.CartId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Photo).WithMany().HasForeignKey(x => x.PhotoId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.CartId, x.PhotoId, x.ProductId }).IsUnique();
        });

        b.Entity<Order>(e =>
        {
            e.Property(x => x.Reference).HasMaxLength(20);
            e.HasIndex(x => x.Reference).IsUnique();
            e.Property(x => x.StudentName).HasMaxLength(200);
            e.Property(x => x.ParentName).HasMaxLength(200);
            e.Property(x => x.Relation).HasMaxLength(50);
            e.Property(x => x.Phone).HasMaxLength(30);
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.ShippingAddress).HasMaxLength(1000);
            e.Property(x => x.Note).HasMaxLength(1000);
            e.Property(x => x.Total).HasPrecision(12, 2);
            e.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<OrderLine>(e =>
        {
            e.Property(x => x.UnitPrice).HasPrecision(10, 2);
            e.Property(x => x.GalleryName).HasMaxLength(400);
            e.Property(x => x.SchoolName).HasMaxLength(200);
            e.Property(x => x.PhotoNumber).HasMaxLength(64);
            e.Property(x => x.ProductName).HasMaxLength(100);
            e.Ignore(x => x.LineTotal);
        });

        b.Entity<ContactMessage>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200);
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.Phone).HasMaxLength(30);
            e.Property(x => x.Message).HasMaxLength(4000);
        });

        // Legacy price list (okul.aspx): 15×23 = 5 TL, 20×30 = 15 TL, 50×70 = 70 TL.
        // Admins change prices in the UI; these are only the initial rows.
        b.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Baskı 15×23", Price = 5m, SortOrder = 1 },
            new Product { Id = 2, Name = "Baskı 20×30", Price = 15m, SortOrder = 2 },
            new Product { Id = 3, Name = "Poster 50×70", Price = 70m, SortOrder = 3 });

        if (Database.IsSqlite())
        {
            // SQLite cannot order or compare DateTimeOffset natively; store as UTC ticks.
            foreach (var entity in b.Model.GetEntityTypes())
            {
                foreach (var prop in entity.GetProperties()
                             .Where(p => p.ClrType == typeof(DateTimeOffset) || p.ClrType == typeof(DateTimeOffset?)))
                {
                    prop.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.DateTimeOffsetToBinaryConverter());
                }
            }
        }
    }
}
