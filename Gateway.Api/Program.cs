using ECommerceSharedLibrary.DependencyInjection;
using Gateway.Api.Middleware;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false,
    reloadOnChange: true);
builder.Services.AddOcelot()
    .AddCacheManager(x => x.WithDictionaryHandle());
JwtAuthenticationScheme.AddJwtAuthenticationScheme(builder.Services, builder.Configuration);
builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

var app = builder.Build();
app.UseCors();
app.UseHttpsRedirection();
app.UseMiddleware<AttachSignatureToRequest>();
app.UseOcelot().Wait();

app.Run();

