using System.ComponentModel.DataAnnotations;

namespace Warehouse.Api.Models;

public class Product
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    [StringLength(50)]
    public string SKU { get; set; } = string.Empty;
    
    [Required]
    [Range(0, double.MaxValue)]
    public double QuantityInStock { get; set; }
}