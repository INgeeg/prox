
using System.Reflection;
using AvroSpecific;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;

class KafkaConsumer: IEventConsumer
{
    private readonly IConfiguration _configuration;

    public KafkaConsumer(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async IAsyncEnumerable<T> ConsumeAsync<T>(string topic, string group)
    { 
        string confluentEndpoint = _configuration.GetSection("Confluent:Producer:Endpoint").Value;
        string confluentUsername = _configuration.GetSection("Confluent:Producer:SaslUsername").Value;
        string confluentPassword = _configuration.GetSection("Confluent:Producer:SaslPassword").Value;
        string confluentSchemaRegistryUrl = _configuration.GetSection("Confluent:Producer:SchemaRegistryUrl").Value;
        string confluentSchemaRegistryBasicAuthUserInfo =_configuration.GetSection("Confluent:Producer:SchemaRegistryUrlBasicAuth").Value;
     
        var consumerConfig = new ConsumerConfig()
        {
            BootstrapServers = confluentEndpoint,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = confluentUsername,
            SaslPassword = confluentPassword,
            GroupId = group,
            AutoOffsetReset = AutoOffsetReset.Earliest 
        };
        
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = confluentSchemaRegistryUrl,
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo, 
            BasicAuthUserInfo = confluentSchemaRegistryBasicAuthUserInfo
        };
        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        using var consumer = new ConsumerBuilder<string, T>(consumerConfig)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(new AvroDeserializer<T>(schemaRegistry).AsSyncOverAsync())
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .Build();
        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        consumer.Subscribe(topic);

        Console.WriteLine("Consuming messages from topic: " + topic + ", broker(s): " + confluentEndpoint);
 
        
        while (!cts.IsCancellationRequested)
        {
            var msg = consumer.Consume(cts.Token);
            yield return (T) Convert.ChangeType(msg.Message.Value, typeof(T));
        }
    }
}
