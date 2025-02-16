using ECommerceSharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ECommerceSharedLibrary.DependencyInjection;

public static class SharedServiceContainer
{
    public static IServiceCollection AddSharedService<TContext>(this IServiceCollection service,
        IConfiguration configuration,string fileName) where TContext : DbContext
    {
        // Add generic database context
        service.AddDbContext<TContext>(opts =>
        {
            opts.UseNpgsql(configuration
                .GetConnectionString("eCommerceConnection"));
            
        });
        
        // configure serilog logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} " +
                                "[{Level:u3}]" +
                                "{message:lj} " +
                                "{NewLine}" +
                                "{Exception}", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        
        // Add JWT Authentication Scheme
        JwtAuthenticationScheme.AddJwtAuthenticationScheme(service, configuration);
        
        return service;
    }

    public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
    {
        // use global exception
        app.UseMiddleware<GlobalException>();
        // register needed api calls
        app.UseMiddleware<ListenToOnlyApiGateway>();

        return app;
    }
}