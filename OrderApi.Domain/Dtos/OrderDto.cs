using System.ComponentModel.DataAnnotations;

namespace OrderApi.Domain.Entities;

public record OrderDto(
    int Id, 
    [Required, Range(1,int.MaxValue)] int ProductId,
    [Required,Range(1,int.MaxValue)] int ClientId,
    [Required,Range(1,int.MaxValue)] int PurchaseQuantity,
    DateTime OrderedDate
    );