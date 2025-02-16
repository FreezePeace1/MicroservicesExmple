using System.ComponentModel.DataAnnotations;

namespace OrderApi.Domain.Entities;

public class Order
{
    [Key]
    public int Id { get; set; }
    
    public int ProductId { get; set; }
    public int ClientId { get; set; }
    public int PurchaseQuantity { get; set; }
    public DateTime OrderData { get; set; } = DateTime.UtcNow;
    
}