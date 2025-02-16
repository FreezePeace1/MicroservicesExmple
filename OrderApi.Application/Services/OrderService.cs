using System.Net.Http.Json;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Dtos;
using OrderApi.Domain.Dtos.Conversions;
using OrderApi.Domain.Entities;
using Polly;
using Polly.Registry;

namespace OrderApi.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepositoryInterface;
    private readonly HttpClient _httpClient;
    private readonly ResiliencePipelineProvider<string> _resiliencePipeline;

    public OrderService(IOrderRepository orderRepositoryInterface,HttpClient httpClient,
        ResiliencePipelineProvider<string> resiliencePipeline)
    {
        _orderRepositoryInterface = orderRepositoryInterface;
        _httpClient = httpClient;
        _resiliencePipeline = resiliencePipeline;
    }
    
    //GET PRODUCT
    public async Task<ProductDto> GetProduct(int productId)
    {
        var getProduct = await _httpClient.GetAsync($"/api/Product/{productId}");
        if (!getProduct.IsSuccessStatusCode)
        {
            return null!;
        }

        var product = await getProduct.Content.ReadFromJsonAsync<ProductDto>();

        //! - show that can not be null
        return product!;
    }
    
    //GET USER
    public async Task<AppUserDto> GetUser(int userId)
    {
        var getUser = await _httpClient.GetAsync($"/api/authentication/{userId}");

        if (!getUser.IsSuccessStatusCode)
        {
            return null!;
        }

        var user = await getUser.Content.ReadFromJsonAsync<AppUserDto>();

        return user!;
    }
    
    public async Task<IEnumerable<OrderDto>> GetOrdersByClientId(int clientId)
    {
        var orders = await _orderRepositoryInterface.GetOrdersAsync(x => x.ClientId == clientId);

        if (!orders.Any())
        {
            return null!;
        }
        
        //Convert from entity to dto
        var (_, list) = OrderConversion.FromEntity(null, orders);

        return list!;
    }

    public async Task<OrderDetailsDto> GetOrderDetails(int orderId)
    {
        // prepare order 
        var order = await _orderRepositoryInterface.FindByIdAsync(orderId);

        if (order is null || order!.Id < 0)
        {
            return null!;
        }
        
        //Get retry pipeline
        var retryPipeline = _resiliencePipeline.GetPipeline("my-retry-pipeline");
        
        //prepare product
        var productDto = await retryPipeline.ExecuteAsync(async token =>
            await GetProduct(order.ProductId));
        
        //prepare client
        var appUserDto = await retryPipeline.ExecuteAsync(async token =>
            await GetUser(order.ClientId));
        
        // Populate order details
        return new OrderDetailsDto
        (
            order.Id,
            productDto.Id,
            appUserDto.Id,
            appUserDto.Name,
            appUserDto.Email,
            appUserDto.Address,
            appUserDto.TelephoneNumber,
            productDto.Name,
            order.PurchaseQuantity,
            productDto.Price,
            productDto.Price * order.PurchaseQuantity,
            order.OrderData
        );
    }
}