using Confluent.Kafka;
using Confluent.SchemaRegistry;
using KafkaConsumer.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaConsumer.Confluent;

/// <summary>
/// DI for ConfluentKafkaEventConsumer classes
/// </summary>
public static class ConfluentKafkaEventConsumerExtensions
{
    /// <summary>
    /// Configures DI for the Azure Kafka Event Consumer.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>An IServiceCollection.</returns>
    public static IServiceCollection AddConfluentKafkaEventConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConsumerConfig>(configuration.GetSection("ConfluentCluster"));
        services.Configure<SchemaRegistryConfig>(configuration.GetSection("ConfluentSchemaRegistry"));
        //services.AddSingleton<IEventConsumerFactory, ConfluentKafkaEventConsumerFactory>();
        services.AddSingleton<IEventConsumerFactory>(provider =>
            new ConfluentKafkaEventConsumerFactory(
                provider.GetRequiredService<IOptions<ConsumerConfig>>(),
                provider.GetRequiredService<IOptions<SchemaRegistryConfig>>(),
                provider.GetRequiredService<ILogger<ConfluentKafkaEventConsumerFactory>>()
            ));

        return services;
    }
}
