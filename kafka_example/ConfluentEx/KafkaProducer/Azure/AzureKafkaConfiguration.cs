public class AzureKafkaConfiguration
{
    public string AzureClientId { get; set; }
    public string AzureTenantId { get; set; }
    public string AzureClientSecret { get; set; }
    public string AzureSchemaGroup { get; set; }
    
    public string SaslUsername { get; set; }
    public string SaslPassword { get; set; }
    public string Endpoint { get; set; }
    public string SchemaRegistryUrl { get; set; }
    public string SchemaRegistryUrlBasicAuth { get; set; }
    public string TopicName { get; set; }
}