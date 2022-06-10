

using System.Text;
using Azure.Data.SchemaRegistry;
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Confluent.Kafka;
using Microsoft.Azure.Data.SchemaRegistry.ApacheAvro;
using Microsoft.Extensions.Options;

namespace KafkaProducer.Azure;
public class AzureAvroSerializer<T> : IAsyncSerializer<T>
{
    private readonly IOptions<AzureSchemaRegistryConfiguration> _options;
    private readonly SchemaRegistryAvroSerializer _serializer;

    public AzureAvroSerializer(IOptions<AzureSchemaRegistryConfiguration> options)
    {
        _options = options;
        var credentials = new ClientSecretCredential(_options.Value.AzureTenantId,_options.Value.AzureClientId,_options.Value.AzureClientSecret);
        this._serializer = new SchemaRegistryAvroSerializer(
            new SchemaRegistryClient(
                _options.Value.AzureSchemaRegistryUrl,
                credentials),
            _options.Value.AzureSchemaGroup,
            new SchemaRegistryAvroSerializerOptions()
            {
                AutoRegisterSchemas = true
            });
    }
    // public AzureAvroSerializer(string schemaRegistryUrl,string schemaGroup,string tenantId, string clientId, string clientSecret, Boolean autoRegisterSchemas = false)
    // {
    //     var credentials = new ClientSecretCredential(tenantId,clientId,clientSecret);
    //     this._serializer = new SchemaRegistryAvroSerializer(
    //         new SchemaRegistryClient(
    //             schemaRegistryUrl,
    //             credentials),
    //         schemaGroup,
    //         new SchemaRegistryAvroSerializerOptions()
    //         {
    //             AutoRegisterSchemas = autoRegisterSchemas
    //         });
    // }

    public async Task<byte[]> SerializeAsync(T data, SerializationContext context)
    {
        
        EventData eventData =   (EventData) await _serializer.SerializeAsync(data, messageType: typeof(EventData));
        context.Headers.Add("schema_id", Encoding.ASCII.GetBytes(eventData.ContentType));
        
        return eventData.EventBody.ToArray();
    }
}