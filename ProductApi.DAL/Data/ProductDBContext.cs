using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;

namespace ProductApi.DAL.Data;

public class ProductDBContext : DbContext
{
    public ProductDBContext(DbContextOptions<ProductDBContext> opts) : base(opts)
    {
        
    }
    
    public DbSet<Product> Products { get; set; }
}