using System.Linq.Expressions;
using ECommerceSharedLibrary.Logs;
using ECommerceSharedLibrary.Response;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.DAL.Data;
using ProductApi.Domain.Entities;

namespace ProductApi.DAL.Repositories;

public class ProductRepository : IProductService
{
    private readonly ProductDBContext _context;

    public ProductRepository(ProductDBContext context)
    {
        _context = context;
    }

    public async Task<Response> CreateAsync(Product entity)
    {
        if (entity is null)
        {
            return new Response
            (
                "Enter the values for entities!"
            );
        }

        try
        {
            var product = await GetByAsync(x => x.ProductName!.Equals(entity.ProductName));

            if (product is not null && !string.IsNullOrEmpty(product.ProductName))
            {
                return new Response
                (
                    $"{product.ProductName} already exists!"
                );
            }

            await _context.Products.AddAsync(entity);

            await _context.SaveChangesAsync();

            return new Response
            (
                $"Product {entity.ProductName} is added successfully!",
                true
            );
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);

            return new Response("Error in adding new product");
        }
    }

    public async Task<Response> UpdateAsync(Product entity)
    {
        try
        {
            var existedProduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == entity.Id);

            if (existedProduct is null)
            {
                return new Response(
                    "Can not find the product"
                );
            }

            var newProduct = await _context.Products.Where(x => x.Id == existedProduct.Id)
                .ExecuteUpdateAsync(x => 
                x
                    .SetProperty(p => p.ProductName,entity.ProductName)
                    .SetProperty(p => p.Price,entity.Price)
                    .SetProperty(p => p.Quantity,entity.Quantity)
                    .SetProperty(p => p.Id,entity.Id)
                );

            return new Response($"Product named {entity.ProductName} is updated" +
                                $"successfully",true);
            
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);

            return new Response("Error in updating the product");
        }
    }

    public async Task<Response> DeleteAsync(Product entity)
    {
        if (entity is null)
        {
            return new Response
            (
                "Enter the values for entities!"
            );
        }

        try
        {
            var existedProduct = await FindByIdAsync(entity.Id);

            if (existedProduct is null)
            {
                return new Response
                (
                    $"Can not find product named {entity.ProductName}"
                );
            }

            _context.Products.Remove(existedProduct);
            await _context.SaveChangesAsync();

            return new Response
            (
                $"{existedProduct.ProductName} is removed successfully!",
                true
            );
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);

            return new Response("Error in deleting product");
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        try
        {
            var products = await _context.Products.AsNoTracking().ToListAsync();

            return products;
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new Exception($"{e.Message}" +
                                $"Can not get all products");
        }
    }

    public async Task<Product> FindByIdAsync(int id)
    {
        if (id < 0)
        {
            throw new ArgumentException("Id can not be less then zero!");
        }

        try
        {
            var existedProduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (existedProduct is not null)
            {
                return existedProduct;
            }

            return null!;
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new Exception($"{e.Message}" +
                                $"Can not find product");
        }
    }

    public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
    {
        try
        {
            var product = await _context.Products.Where(predicate).FirstOrDefaultAsync();

            return product is not null ? product : null!;
        }
        catch (Exception e)
        {
            LogException.LogExceptions(e);
            throw new Exception($"{e.Message}" +
                                $"Can not get by predicate product");
        }
    }
}