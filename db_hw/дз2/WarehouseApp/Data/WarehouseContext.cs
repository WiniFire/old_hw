using Microsoft.EntityFrameworkCore;
using WarehouseApp.Models;

namespace WarehouseApp.Data;

public class WarehouseContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string databasePath = Path.Combine(AppContext.BaseDirectory, "warehouse.db");
        optionsBuilder.UseSqlite($"Data Source={databasePath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(product => product.Id);

            entity.Property(product => product.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(product => product.Category)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(product => product.Price)
                .HasPrecision(10, 2);

            entity.ToTable(tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_Products_Quantity", "Quantity >= 0");
                tableBuilder.HasCheckConstraint("CK_Products_Price", "Price >= 0");
            });

            entity.HasData(
                new Product { Id = 1, Name = "Ноутбук", Category = "Электроника", Quantity = 5, Price = 35000m },
                new Product { Id = 2, Name = "Клавиатура", Category = "Аксессуары", Quantity = 12, Price = 1200m },
                new Product { Id = 3, Name = "Мышь", Category = "Аксессуары", Quantity = 20, Price = 650m }
            );
        });
    }
}
