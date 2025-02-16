using ECommerceSharedLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using OrderApi.Domain.Dtos.Conversions;
using OrderApi.Domain.Entities;

namespace OrderApi.Api.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderService _orderService;

    public OrderController(IOrderRepository orderRepository, IOrderService orderService)
    {
        _orderRepository = orderRepository;
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var orders = await _orderRepository.GetAllAsync();

        if (!orders.Any())
        {
            var (_, list) = OrderConversion.FromEntity(null, orders);

            return list.Any() ? Ok(list) : NotFound();
        }

        return NotFound("Can not find any data in DB");
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var order = await _orderRepository.FindByIdAsync(id);

        if (order is null)
        {
            return NotFound("Order is not found");
        }

        var (_order, _) = OrderConversion.FromEntity(order, null);

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Response>> CreateOrder(OrderDto orderDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Can not submit this data");
        }

        var entity = OrderConversion.ToEntity(orderDto);

        var response = await _orderRepository.CreateAsync(entity);

        return response.Flag ? Ok(response) : BadRequest(response);
    }

    [HttpPut]
    public async Task<ActionResult<Response>> UpdateOrder(OrderDto orderDto)
    {
        var entity = OrderConversion.ToEntity(orderDto);

        var updatedOrder = await _orderRepository.UpdateAsync(entity);

        return updatedOrder.Flag ? Ok(updatedOrder) : BadRequest(updatedOrder);
    }

    [HttpDelete]
    public async Task<ActionResult<Response>> DeleteOrder(OrderDto orderDto)
    {
        var entity = OrderConversion.ToEntity(orderDto);

        var response = await _orderRepository.DeleteAsync(entity);

        return response.Flag ? Ok(response) : BadRequest(response);
    }

    [HttpGet("client/{clientId:int}")]
    public async Task<ActionResult<OrderDto>> GetClientOrders(int clientId)
    {
        if (clientId < 0)
        {
            return BadRequest("Invalid data");
        }

        var response = await _orderService.GetOrdersByClientId(clientId);
            
        return !response.Any() ? NotFound(response) : Ok(response);
    }

    [HttpGet("details/{orderId:int}")]
    public async Task<ActionResult<OrderDto>> GetOrderDetails(int orderId)
    {
        if (orderId < 0)
        {
            return BadRequest("Invalid data");
        }

        var orderDetails = await _orderService.GetOrderDetails(orderId);

        return orderDetails is not null ? Ok(orderDetails) : NotFound("No data in here");
    }
}