using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaProducer.Azure;
class AzureKafkaEventProducerFactory : IEventProducerFactory
{
    private readonly IOptions<AzureSchemaRegistryConfiguration> _schemaRegistryOptions;
    private readonly ILogger _logger;
    private readonly IOptions<ProducerConfig> _produceConfigOptions;


    public AzureKafkaEventProducerFactory(
        IOptions<AzureSchemaRegistryConfiguration> schemaRegistryOptions,
        ILogger logger,
        IOptions<ProducerConfig> produceConfigOptions)
    {
        _schemaRegistryOptions = schemaRegistryOptions;
        _logger = logger;
        _produceConfigOptions = produceConfigOptions;
    }
    public IEventProducer<TKey,TValue> CreateEventProducer<TKey,TValue>()
    {
        var valueSerializer = new AzureAvroSerializer<TValue>(_schemaRegistryOptions);
        var producer = new ProducerBuilder<TKey,TValue>(_produceConfigOptions.Value)
            //.SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(valueSerializer)
            .SetErrorHandler((_, e) => _logger.LogError($"Error: {e.Reason}")) // TODO: ???
            .Build();

        return new AzureKafkaEventProducer<TKey,TValue>(producer);
    }
}