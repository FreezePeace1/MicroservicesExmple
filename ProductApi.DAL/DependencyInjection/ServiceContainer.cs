using ECommerceSharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.DAL.Data;
using ProductApi.DAL.Repositories;

namespace ProductApi.DAL.DependencyInjection;

public static class ServiceContainer
{
   public static IServiceCollection AddInfrastructureService(this IServiceCollection service,IConfiguration configuration)
   {
      //Add database connectivity
      //Add authentication scheme
      SharedServiceContainer.AddSharedService<ProductDBContext>(service,configuration,
         configuration["MySerilog:FileName"]!);

      service.AddScoped<IProductService, ProductRepository>();

      return service;
   }

   public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
   {
      //Register middleware such as:
      // Global Exception to handle external errors
      // Listen to only api gateway to block all outsider calls
      SharedServiceContainer.UseSharedPolicies(app);

      return app;
   }
}