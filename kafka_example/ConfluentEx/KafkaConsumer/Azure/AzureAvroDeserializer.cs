using System.Text;
using Azure.Data.SchemaRegistry;
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Confluent.Kafka;
using KafkaProducer.Azure;
using Microsoft.Azure.Data.SchemaRegistry.ApacheAvro;
using Microsoft.Extensions.Options;

namespace KafkaConsumer.Azure;

public class AzureAvroDeserializer<T> : IAsyncDeserializer<T>
{
    private readonly IOptions<AzureSchemaRegistryConfiguration> _schemaConfigOptions;
    private readonly SchemaRegistryAvroSerializer serializer;

    public AzureAvroDeserializer(IOptions<AzureSchemaRegistryConfiguration> schemaConfigOptions)
    {
        _schemaConfigOptions = schemaConfigOptions;
        var credentials = new ClientSecretCredential(_schemaConfigOptions.Value.AzureTenantId,_schemaConfigOptions.Value.AzureClientId,_schemaConfigOptions.Value.AzureClientSecret);
        this.serializer = new SchemaRegistryAvroSerializer(new SchemaRegistryClient(_schemaConfigOptions.Value.AzureSchemaRegistryUrl, credentials), _schemaConfigOptions.Value.AzureSchemaGroup);
    }

    public Task<T> DeserializeAsync(ReadOnlyMemory<byte> data,
        bool isNull, SerializationContext context)
    {
   
        byte[] result;
        context.Headers.TryGetLastBytes("schema_id", out result);
        var msg = new EventData()
        {
            Data = BinaryData.FromBytes(data),
            ContentType = Encoding.ASCII.GetString(result, 0, result.Length)
        };
        T deserialized = (T) serializer.Deserialize(msg, typeof(T), CancellationToken.None);

        return Task.FromResult(deserialized);
    }
}