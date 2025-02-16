using ECommerceSharedLibrary.Logs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Application.Services;
using Polly;
using Polly.Retry;

namespace OrderApi.Application.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddApplicationServiceAndRetryStrategyOpts(this IServiceCollection service,
        IConfiguration configuration)
    {
        service.AddHttpClient<IOrderService,OrderService>(opts =>
        {
            opts.BaseAddress = new Uri(configuration["ApiGateway:BaseAddress"]!);
            opts.Timeout = TimeSpan.FromSeconds(1);
        });
        
        //retry strategy
        var retryStrategy = new RetryStrategyOptions()
        {
            ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
            BackoffType = DelayBackoffType.Constant,
            UseJitter = true,
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(500),
            OnRetry = args =>
            {
                string message = $"onRetry attempt: {args.AttemptNumber}" +
                                 $"Outcome {args.Outcome}";
                LogException.LogToConsole(message);
                LogException.LogToDebugger(message);
                
                return ValueTask.CompletedTask;
            }
        };
        
        // use retry strategy
        service.AddResiliencePipeline("my-retry-pipeline", builder =>
        {
            builder.AddRetry(retryStrategy);
        });


        return service;
    }
}