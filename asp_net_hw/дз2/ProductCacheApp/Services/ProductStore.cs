using ProductCacheApp.Models;

namespace ProductCacheApp.Services;

public class ProductStore
{
    private readonly object _sync = new();
    private readonly Dictionary<int, Product> _products = new()
    {
        [123] = new Product(123, "Блокнот", 120m, 1)
    };

    public Product? Find(int id)
    {
        lock (_sync)
        {
            return _products.GetValueOrDefault(id);
        }
    }

    public Product? Update(int id, string name, decimal price)
    {
        lock (_sync)
        {
            if (!_products.TryGetValue(id, out var current)) return null;
            name = name.Trim();
            price = decimal.Round(price, 2);
            if (current.Name == name && current.Price == price) return current;

            var updated = current with { Name = name, Price = price, Version = current.Version + 1 };
            _products[id] = updated;
            return updated;
        }
    }
}
