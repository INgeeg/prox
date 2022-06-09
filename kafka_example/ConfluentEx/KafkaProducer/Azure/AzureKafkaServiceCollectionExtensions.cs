using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaProducer.Azure;

public static class AzureKafkaServiceCollectionExtensions
{
    public static IServiceCollection AddAzureKafkaProducer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureKafkaConfiguration>(configuration.GetSection("EventHub:Producer"));
        services.AddSingleton<IEventProducerFactory>(provider => 
            new AzureKafkaEventProducerFactory(
                provider.GetRequiredService<IOptions<AzureKafkaConfiguration>>(),
                provider.GetRequiredService<ILogger<ConfluentKafkaEventProducerFactory>>()
            )
        );
        
        return services;
    }

}