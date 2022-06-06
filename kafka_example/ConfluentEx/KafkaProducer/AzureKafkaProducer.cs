
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

class AzureKafkaProducer: IEventProducer
{
    private readonly IConfiguration _configuration;

    public AzureKafkaProducer(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task ProduceAsync<T>(string topic, string key, T message)
    {
        try
        {
            string azureEndpoint = _configuration.GetSection("EventHub:Producer:Endpoint").Value;
            string azureEventHubCredentials = _configuration.GetSection("EventHub:Producer:SaslPassword").Value;
            string azureSchemaRegistryUrl = _configuration.GetSection("EventHub:Producer:SchemaRegistryUrl").Value;
            string azureSaslUserName = _configuration.GetSection("EventHub:Producer:SaslUsername").Value;
            string azureSchemaGroup = _configuration.GetSection("EventHub:Producer:AzureSchemaGroup").Value;
            string azureTenantId = _configuration.GetSection("EventHub:Producer:AzureTenantId").Value;
            string azureClientId = _configuration.GetSection("EventHub:Producer:AzureClientId").Value;
            string azureClientSecret = _configuration.GetSection("EventHub:Producer:AzureClientSecret").Value;
       
            
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = azureEndpoint,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = azureSaslUserName,
                SaslPassword = azureEventHubCredentials
            };
            
            using var producer = new ProducerBuilder<string, T>(producerConfig)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(new AzureAvroSerializer<T>(azureSchemaRegistryUrl,azureSchemaGroup,azureTenantId,azureClientId,azureClientSecret,true))
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                .Build();
        
            Console.WriteLine("Sending 1 messages to topic: " + topic + ", broker(s): " + azureEndpoint);
            var deliveryReport = await producer.ProduceAsync(topic, new Message<string, T> {Key = key, Value = message});
            Console.WriteLine(string.Format("Message {0} sent (value: '{1}')", key, message));
 
        }
        catch (Exception e)
        {
            Console.WriteLine(string.Format("Exception Occurred - {0}", e.Message));
        }
        
    }

 
}
