using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaConsumer.Confluent;

public class ConfluentKafkaEventConsumerFactory : IEventConsumerFactory
{
    private readonly IOptions<ConsumerConfig> _consumerConfigOptions;
    private readonly IOptions<SchemaRegistryConfig> _schemaRegistryOptions;
    private readonly ILogger _logger;

    public ConfluentKafkaEventConsumerFactory(
        IOptions<ConsumerConfig> consumerConfigOptions,
        IOptions<SchemaRegistryConfig> schemaRegistryOptions,
        ILogger logger)
    {
        _consumerConfigOptions = consumerConfigOptions;
        _schemaRegistryOptions = schemaRegistryOptions;
        _logger = logger;
    }

    
    public IEventConsumer<TValue> CreateEventConsumer<TValue>()
    {
        var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryOptions.Value);

        var consumer = new ConsumerBuilder<string, TValue>(_consumerConfigOptions.Value)
            //.SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(new AvroDeserializer<TValue>(schemaRegistry).AsSyncOverAsync())
            .SetErrorHandler((_, e) => _logger.LogError($"Error: {e.Reason}"))
            .Build();

        return new ConfluentKafkaEventConsumer<TValue>(consumer);
    }
}
