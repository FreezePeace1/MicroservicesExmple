using System.ComponentModel.DataAnnotations;

namespace ProductApi.Domain.Entities;

public class Product
{
    [Key]
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}