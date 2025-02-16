using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ECommerceSharedLibrary.DependencyInjection;

public static class JwtAuthenticationScheme
{
    public static IServiceCollection AddJwtAuthenticationScheme(this IServiceCollection service,
        IConfiguration configuration)
    {
        // Добавим JWT service
        service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer("Bearer", opts =>
            {
                var key = Encoding.UTF8.GetBytes(configuration.GetSection("Authentication:Key").Value!);
                string issuer = configuration.GetSection("Authentication:Issuer").Value!;
                string audience = configuration.GetSection("Authentication:Audience").Value!;

                opts.RequireHttpsMetadata = false;
                opts.SaveToken = true;
                opts.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

        return service;
    }
}