using Confluent.Kafka;
using Confluent.SchemaRegistry;
using KafkaProducer.Confluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaProducer.Confluent;

public static class ConfluentKafkaEventServiceCollectionExtensions
{ 
    public static IServiceCollection AddConfluentKafkaProducer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ProducerConfig>(configuration.GetSection("ConfluentCluster"));
        services.Configure<SchemaRegistryConfig>(configuration.GetSection("ConfluentSchemaRegistry"));
        //services.AddSingleton<IEventProducerFactory, ConfluentKafkaEventProducerFactory>();
        services.AddSingleton<IEventProducerFactory>(provider => 
            new ConfluentKafkaEventProducerFactory(
                provider.GetRequiredService<IOptions<ProducerConfig>>(),
                provider.GetRequiredService<ILogger<ConfluentKafkaEventProducerFactory>>(),
                provider.GetRequiredService<IOptions<SchemaRegistryConfig>>()
            )
        );
        
        return services;
    }

}