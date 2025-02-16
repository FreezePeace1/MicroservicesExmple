using System.Linq.Expressions;
using ECommerceSharedLibrary.Interfaces;
using OrderApi.Domain.Entities;

namespace OrderApi.Application.Interfaces;

public interface IOrderRepository : IGenericInterface<Order>
{
    Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order,bool>> predicate);
}