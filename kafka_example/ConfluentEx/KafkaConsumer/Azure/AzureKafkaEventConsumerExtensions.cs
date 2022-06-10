using Confluent.Kafka;
using KafkaProducer.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaConsumer.Azure;

/// <summary>
/// DI for AzureKafkaEventConsumer classes
/// </summary>
public static class AzureKafkaEventConsumerExtensions
{
    /// <summary>
    /// Configures DI for the Azure Kafka Event Consumer.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>An IServiceCollection.</returns>
    public static IServiceCollection AddAzureKafkaEventConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureSchemaRegistryConfiguration>(configuration.GetSection("AzureSchemaRegistry"));
        services.Configure<ConsumerConfig>(configuration.GetSection("AzureEventHub"));
        //services.AddSingleton<IEventConsumerFactory, AzureKafkaEventConsumerFactory>();
        services.AddSingleton<IEventConsumerFactory>(provider =>
            new AzureKafkaEventConsumerFactory(
                provider.GetRequiredService<IOptions<AzureSchemaRegistryConfiguration>>(),
                provider.GetRequiredService<IOptions<ConsumerConfig>>(),
                provider.GetRequiredService<ILogger<AzureKafkaEventConsumerFactory>>()
            ));

        return services;
    }
}
