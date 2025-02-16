using Microsoft.EntityFrameworkCore;
using OrderApi.Domain.Entities;

namespace OrderApi.DAL.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> opts) : base(opts)
    {
        
    }
    
    public DbSet<Order> Orders { get; set; }
}