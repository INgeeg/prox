
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaProducer.Confluent;
class ConfluentKafkaEventProducerFactory : IEventProducerFactory
{
    private readonly IOptions<ProducerConfig> _producerConfigOptions;
    private readonly ILogger _logger;
    private readonly IOptions<SchemaRegistryConfig> _schemaRegistryConfigOptions;


    public ConfluentKafkaEventProducerFactory(
        IOptions<ProducerConfig> producerConfigOptions,
        ILogger logger,
        IOptions<SchemaRegistryConfig> schemaRegistryConfigOptions
        )
    {
        _producerConfigOptions = producerConfigOptions;
        _logger = logger;
        _schemaRegistryConfigOptions = schemaRegistryConfigOptions;
    }
    
    
    
    
    public IEventProducer<TKey, TValue> CreateEventProducer<TKey, TValue>()
    {
        var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfigOptions.Value);
        var producer =  new ProducerBuilder<TKey, TValue>(_producerConfigOptions.Value)
            .SetValueSerializer(new AvroSerializer<TValue>(schemaRegistry))
            .Build();

        return new ConfluentKafkaEventProducer<TKey, TValue>(producer);
    }
}