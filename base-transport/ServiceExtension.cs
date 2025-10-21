using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace base_transport;

public static class ServiceExtension
{
    public static IServiceCollection AddTransportationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("RabbitMQ");
        services.Configure<MessagingCredentials>(section);
        
        services.AddSingleton<IBasicMessagingService, BasicMessagingService>();
        return services;
    }
}