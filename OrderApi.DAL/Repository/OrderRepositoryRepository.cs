using System.Linq.Expressions;
using ECommerceSharedLibrary.Logs;
using ECommerceSharedLibrary.Response;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.DAL.Data;
using OrderApi.Domain.Entities;

namespace OrderApi.DAL.Repository;

public class OrderRepositoryRepository : IOrderRepository
{
    private readonly OrderDbContext _dbContext;

    public OrderRepositoryRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Response> CreateAsync(Order entity)
    {
        if (entity is null)
        {
            return new Response("Please enter the values for entity!");
        }
        
        try
        {
            await _dbContext.Orders.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return new Response($"Order created successfully",true);
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);

            return new Response($"{e.Message}");
        }
    }

    public async Task<Response> UpdateAsync(Order entity)
    {
        if (entity is null)
        {
            return new Response("Please enter the values for entity!");
        }
        
        try
        {
            var existedOrder = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == entity.Id);

            if (existedOrder is null)
            {
                return new Response($"Can not find order with {entity.Id} Id");
            }
            
            var newOrder = await _dbContext.Orders.Where(x => x.Id == entity.Id)
                .ExecuteUpdateAsync(x =>
                    x.SetProperty(o => o.Id, entity.Id)
                        .SetProperty(o => o.ClientId, entity.ClientId)
                        .SetProperty(o => o.OrderData, entity.OrderData)
                        .SetProperty(o => o.ProductId, entity.ProductId)
                        .SetProperty(o => o.PurchaseQuantity, entity.PurchaseQuantity)
                );

            return new Response($"Order created successfully",true);
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);

            return new Response($"{e.Message}");
        }
    }

    public async Task<Response> DeleteAsync(Order entity)
    {
        if (entity is null)
        {
            return new Response("Please enter the values for entity!");
        }
        
        try
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == entity.Id);

            if (order is null)
            {
                return new Response($"Can not find order with {order.Id} Id");
            }

            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();

            return new Response($"Order deleted successfully!",true);

        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);

            return new Response($"{e.Message}");
        }
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        
        
        try
        {
            var orders = await _dbContext.Orders.AsNoTracking().ToListAsync();

            return orders is not null ? orders : null!;
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);

            return null!;
        }
    }

    public async Task<Order> FindByIdAsync(int id)
    {
        if (id < 0)
        {
            throw new ArgumentException("Enter id more or equal then zero!");
        }
        
        try
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id);

            return order is not null ? order : null!;
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);

            return null!;
        }
    }

    public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
    {
        try
        {
            var order = await _dbContext.Orders.Where(predicate).FirstOrDefaultAsync();

            return order is not null ? order : null!;
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);

            return null!;
        };
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
    {
        try
        {
            var orders = await _dbContext.Orders.Where(predicate).ToListAsync();

            return orders is not null ? orders : null!;
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);

            return null!;
        }
    }
}