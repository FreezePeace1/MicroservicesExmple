using System.Linq.Expressions;

namespace ECommerceSharedLibrary.Interfaces;

public interface IGenericInterface<T> where T : class
{
    Task<Response.Response> CreateAsync(T entity);
    Task<Response.Response> UpdateAsync(T entity);
    Task<Response.Response> DeleteAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> FindByIdAsync(int id);
    Task<T> GetByAsync(Expression<Func<T,bool>> predicate);
}