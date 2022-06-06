
using AvroSpecific;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;

class KafkaProducer : IEventProducer
{
    private readonly IConfiguration _configuration;

    public KafkaProducer(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task ProduceAsync<T>(string topic, string key, T message)
    {
        string confluentEndpoint = _configuration.GetSection("Confluent:Producer:Endpoint").Value;
        string confluentUsername = _configuration.GetSection("Confluent:Producer:SaslUsername").Value;
        string confluentPassword = _configuration.GetSection("Confluent:Producer:SaslPassword").Value;
        string confluentSchemaRegistryUrl = _configuration.GetSection("Confluent:Producer:SchemaRegistryUrl").Value;
        string confluentSchemaRegistryBasicAuthUserInfo =_configuration.GetSection("Confluent:Producer:SchemaRegistryUrlBasicAuth").Value;

        try
        {
            var config = new ProducerConfig
            {
                BootstrapServers = confluentEndpoint,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = confluentUsername,
                SaslPassword = confluentPassword
            };



            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = confluentSchemaRegistryUrl,
                BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo,
                BasicAuthUserInfo = confluentSchemaRegistryBasicAuthUserInfo
            };
            using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
            using var producer = new ProducerBuilder<string, T>(config)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(new AvroSerializer<T>(schemaRegistry))
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                .Build();
            Console.WriteLine("Sending 1 messages to topic: " + topic + ", broker(s): " + confluentEndpoint);
          
            var deliveryReport = await producer.ProduceAsync(topic,
                new Message<string, T> {Key = key, Value = message});
            Console.WriteLine(string.Format("Message {0} sent (value: '{1}')", key, message));
        }
        catch (Exception e)
        {
            Console.WriteLine(string.Format("Exception Occurred - {0}", e.Message));
        }
    }
}
