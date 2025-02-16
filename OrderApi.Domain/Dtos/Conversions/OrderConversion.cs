using OrderApi.Domain.Entities;

namespace OrderApi.Domain.Dtos.Conversions;

public static class OrderConversion
{
    public static Order ToEntity(OrderDto orderDto)
        => new Order()
        {
            ClientId = orderDto.ClientId,
            Id = orderDto.Id,
            OrderData = orderDto.OrderedDate,
            ProductId = orderDto.ProductId,
            PurchaseQuantity = orderDto.PurchaseQuantity
        };

    // ? - could be null
    public static (OrderDto?, IEnumerable<OrderDto>?) FromEntity(Order? order,
        IEnumerable<Order>? orders)
    {
        if (order is not null || orders is null)
        {
            var singleOrder = new OrderDto(
                order.Id,order.ProductId,
                order.ClientId,order.PurchaseQuantity,
                order.OrderData
                );
            
            return (singleOrder,null);
        }

        if (orders is not null || order is null)
        {
            var ordersDto = orders.Select(dto => new OrderDto
                (
                    dto.Id,
                    dto.ProductId,
                    dto.ClientId,
                    dto.PurchaseQuantity,
                    dto.OrderData
                )
            );
            
            return (null, ordersDto);
        }

        return (null, null);
    }
}