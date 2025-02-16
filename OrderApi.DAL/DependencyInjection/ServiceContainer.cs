using ECommerceSharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Interfaces;
using OrderApi.DAL.Data;
using OrderApi.DAL.Repository;

namespace OrderApi.DAL.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureService(
        this IServiceCollection service, IConfiguration configuration)
    {
        SharedServiceContainer.AddSharedService<OrderDbContext>(service,configuration,
            configuration["MySerilog:FileName"]!);

        service.AddScoped<IOrderRepository, OrderRepositoryRepository>();

        return service;
    }

    public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
    {
        SharedServiceContainer.UseSharedPolicies(app);

        return app;
    }
}