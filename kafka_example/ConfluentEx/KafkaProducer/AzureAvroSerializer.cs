

using System.Text;
using Azure.Core;
using Azure.Data.SchemaRegistry;
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Confluent.Kafka;
using Microsoft.Azure.Data.SchemaRegistry.ApacheAvro;

public class AzureAvroSerializer<T> : IAsyncSerializer<T>
{
    private readonly SchemaRegistryAvroSerializer _serializer;
    
    public AzureAvroSerializer(string schemaRegistryUrl,string schemaGroup,string tenantId, string clientId, string clientSecret, Boolean autoRegisterSchemas = false)
    {
        var credentials = new ClientSecretCredential(tenantId,clientId,clientSecret);
        this._serializer = new SchemaRegistryAvroSerializer(
            new SchemaRegistryClient(
                schemaRegistryUrl,
                credentials),
            schemaGroup,
            new SchemaRegistryAvroSerializerOptions()
            {
                AutoRegisterSchemas = autoRegisterSchemas
            });
    }


    public Task<byte[]> SerializeAsync(T data, SerializationContext context)
    {
        
        EventData eventData =  (EventData) _serializer.Serialize(data, messageType: typeof(EventData));
        context.Headers.Add("schema_id", Encoding.ASCII.GetBytes(eventData.ContentType));
        
        return Task.FromResult(eventData.EventBody.ToArray());
    }
}