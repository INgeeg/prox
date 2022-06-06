
using AvroSpecific;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

class AzureKafkaConsumer: IEventConsumer
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;


    public AzureKafkaConsumer(IConfiguration configuration, ILogger logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async IAsyncEnumerable<T> ConsumeAsync<T>(string topic, string group)
    {
        string azureEndpoint = _configuration.GetSection("EventHub:Producer:Endpoint").Value;
        string azureEventHubCredentials = _configuration.GetSection("EventHub:Producer:SaslPassword").Value;
        string azureSchemaRegistryUrl = _configuration.GetSection("EventHub:Producer:SchemaRegistryUrl").Value;
        string azureSaslUserName = _configuration.GetSection("EventHub:Producer:SaslUsername").Value;
        string azureSchemaGroup = _configuration.GetSection("EventHub:Producer:AzureSchemaGroup").Value;
        string azureTenantId = _configuration.GetSection("EventHub:Producer:AzureTenantId").Value;
        string azureClientId = _configuration.GetSection("EventHub:Producer:AzureClientId").Value;
        string azureClientSecret = _configuration.GetSection("EventHub:Producer:AzureClientSecret").Value;
        
        var consumerConfig = new ConsumerConfig()
        {
            BootstrapServers = azureEndpoint,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = azureSaslUserName,
            SaslPassword = azureEventHubCredentials,
            GroupId = group,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, T>(consumerConfig)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(new AzureAvroDeserializer<T>(azureSchemaRegistryUrl, azureSchemaGroup,
                azureTenantId, azureClientId, azureClientSecret).AsSyncOverAsync())
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .Build();
        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        consumer.Subscribe(topic);

        Console.WriteLine("Consuming messages from topic: " + topic + ", broker(s): " + azureEndpoint);

        while (!cts.IsCancellationRequested)
        {
            var msg = consumer.Consume(cts.Token);
            yield return (T) Convert.ChangeType(msg.Message.Value, typeof(T));
        }
    }
}

