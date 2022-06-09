
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

class AzureKafkaEventProducerFactory : IEventProducerFactory
{
    private readonly IOptions<AzureKafkaConfiguration> _options;
    private readonly ILogger _logger;
    
    
    public AzureKafkaEventProducerFactory(
        IOptions<AzureKafkaConfiguration> options,
        ILogger logger)
    {
        _options = options;
        _logger = logger;
    }
    public IEventProducer<T> CreateProducer<T>()
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _options.Value.Endpoint,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = _options.Value.SaslUsername,
            SaslPassword = _options.Value.SaslPassword
        };

        var producer = new ProducerBuilder<string, T>(producerConfig)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(new AzureAvroSerializer<T>(
                _options.Value.SchemaRegistryUrl,
                _options.Value.AzureSchemaGroup,
                _options.Value.AzureTenantId,
                _options.Value.AzureClientId,
                _options.Value.AzureClientSecret,
                true))
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}")) // TODO: ???
            .Build();

        return new AzureKafkaProducer<T>(producer);
    }
}