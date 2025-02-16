using System.ComponentModel.DataAnnotations;

namespace ProductApi.Domain.Dtos;

public record ProductDto([Required]int Id,[Required]string Name,
    [Required,Range(1,int.MaxValue)] int Quantity,
    [Required, DataType(DataType.Currency)] decimal Price);