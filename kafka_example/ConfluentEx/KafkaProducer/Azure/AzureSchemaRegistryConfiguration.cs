namespace KafkaProducer.Azure;
public class AzureSchemaRegistryConfiguration
{
    public string AzureClientId { get; set; }
    public string AzureTenantId { get; set; }
    public string AzureClientSecret { get; set; }
    public string AzureSchemaGroup { get; set; }
    
    public string AzureSchemaRegistryUrl { get; set; }
}