using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.DAL.Data;
using AuthenticationApi.DAL.Repository;
using ECommerceSharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.DAL.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection service,
        IConfiguration configuration)
    {
        SharedServiceContainer.AddSharedService<AuthenticationDbContext>(service,configuration,
            configuration["MySerilog:FileName"]!);

        service.AddScoped<IUser, UserRepository>();
        
        return service;
    }

    public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
    {

        SharedServiceContainer.UseSharedPolicies(app);
        
        return app;
    }
}