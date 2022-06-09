using Confluent.Kafka;

public class ConfluentKafkaConfiguration
{
    public string SaslUsername { get; set; }
    public string SaslPassword { get; set; }
    public string Endpoint { get; set; }
    public string SchemaRegistryUrl { get; set; }
    public string SchemaRegistryUrlBasicAuth { get; set; }
    public string GroupId { get; set; }
    public string TopicName { get; set; }
    public AutoOffsetReset Offset { get; set; }
}