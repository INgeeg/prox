
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

class ConfluentKafkaEventProducerFactory : IEventProducerFactory
{
    private readonly IOptions<ConfluentKafkaConfiguration> _options;
    private readonly ILogger _logger;
    
    
    public ConfluentKafkaEventProducerFactory(
        IOptions<ConfluentKafkaConfiguration> options,
        ILogger logger)
    {
        _options = options;
        _logger = logger;
    }
    public IEventProducer<T> CreateProducer<T>()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _options.Value.Endpoint,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = _options.Value.SaslUsername,
            SaslPassword = _options.Value.SaslPassword
        };

        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = _options.Value.SchemaRegistryUrl,
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
            BasicAuthUserInfo = _options.Value.SchemaRegistryUrlBasicAuth
        };
        var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        var producer =  new ProducerBuilder<string, T>(config)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(new AvroSerializer<T>(schemaRegistry))
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .Build();

        return new ConfluentKafkaProducer<T>(producer);
    }
}