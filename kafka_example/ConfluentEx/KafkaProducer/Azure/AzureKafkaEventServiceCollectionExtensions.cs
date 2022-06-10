
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaProducer.Azure;

public static class AzureKafkaEventServiceCollectionExtensions
{
    public static IServiceCollection AddAzureKafkaProducer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureSchemaRegistryConfiguration>(configuration.GetSection("AzureSchemaRegistry"));
        services.Configure<ProducerConfig>(configuration.GetSection("AzureEventHub"));
        
        //services.AddSingleton<IEventProducerFactory, AzureKafkaEventProducerFactory>();
        services.AddSingleton<IEventProducerFactory>(provider => 
            new AzureKafkaEventProducerFactory(
                provider.GetRequiredService<IOptions<AzureSchemaRegistryConfiguration>>(),
                provider.GetRequiredService<ILogger<AzureKafkaEventProducerFactory>>(),
                provider.GetRequiredService<IOptions<ProducerConfig>>()
            )
        );
        
        return services;
    }

}