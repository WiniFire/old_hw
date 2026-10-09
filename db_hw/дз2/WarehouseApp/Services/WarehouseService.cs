using Microsoft.EntityFrameworkCore;
using WarehouseApp.Data;
using WarehouseApp.Models;

namespace WarehouseApp.Services;

public class WarehouseService
{
    public async Task<List<Product>> GetAllAsync()
    {
        await using var db = new WarehouseContext();
        return await db.Products
            .AsNoTracking()
            .OrderBy(product => product.Id)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        await using var db = new WarehouseContext();
        return await db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Id == id);
    }

    public async Task<Product> CreateAsync(string name, string category, int quantity, decimal price)
    {
        await using var db = new WarehouseContext();

        var product = new Product
        {
            Name = name,
            Category = category,
            Quantity = quantity,
            Price = price
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();
        return product;
    }

    public async Task<bool> UpdateAsync(int id, string name, string category, int quantity, decimal price)
    {
        await using var db = new WarehouseContext();
        Product? product = await db.Products.FindAsync(id);

        if (product is null)
        {
            return false;
        }

        product.Name = name;
        product.Category = category;
        product.Quantity = quantity;
        product.Price = price;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var db = new WarehouseContext();
        Product? product = await db.Products.FindAsync(id);

        if (product is null)
        {
            return false;
        }

        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return true;
    }
}
