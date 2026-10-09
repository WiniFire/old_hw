using System.ComponentModel.DataAnnotations;

namespace ProductCacheApp.Models;

public class UpdateProductRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(typeof(decimal), "0", "1000000")]
    public decimal? Price { get; set; }
}
