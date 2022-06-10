using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using KafkaProducer.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaConsumer.Azure;
public class AzureKafkaEventConsumerFactory : IEventConsumerFactory
{
    private readonly IOptions<AzureSchemaRegistryConfiguration> _schemaRegistryOptions;
    private readonly IOptions<ConsumerConfig> _consumerConfigOptions;
    private readonly ILogger _logger;

    public AzureKafkaEventConsumerFactory(
        IOptions<AzureSchemaRegistryConfiguration> schemaRegistryOptions, 
        IOptions<ConsumerConfig> consumerConfigOptions, 
        ILogger logger)
    {
        _schemaRegistryOptions = schemaRegistryOptions;
        _consumerConfigOptions = consumerConfigOptions;
        _logger = logger;
    }

    public IEventConsumer<TValue> CreateEventConsumer<TValue>()
    {
       IAsyncDeserializer<TValue> deserializer = new AzureAvroDeserializer<TValue>(_schemaRegistryOptions);
       var consumer = new ConsumerBuilder<string, TValue>(_consumerConfigOptions.Value)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(deserializer.AsSyncOverAsync())
            .SetErrorHandler((_, e) => _logger.LogError($"Error: {e.Reason}"))
            .Build();

        return new AzureKafkaEventConsumer<TValue>(consumer);
    }
}
