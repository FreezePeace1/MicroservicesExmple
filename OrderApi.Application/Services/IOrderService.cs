using OrderApi.Domain.Dtos;
using OrderApi.Domain.Entities;

namespace OrderApi.Application.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetOrdersByClientId(int clientId);
    Task<OrderDetailsDto> GetOrderDetails(int orderId);
}