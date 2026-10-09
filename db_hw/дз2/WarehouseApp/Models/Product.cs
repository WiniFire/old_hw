using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Models;

public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0", "999999999.99")]
    public decimal Price { get; set; }
}
