
using Confluent.Kafka;

public class AzureKafkaProducer<T> : IEventProducer<T>
{
    private readonly IProducer<string, T> _producer;

    public AzureKafkaProducer(IProducer<string,T> producer)
    {
        _producer = producer;
    }
    public async Task ProduceAsync(string topicName, string key, T value, CancellationTokenSource cancellationTokenSource)
    {
        var message = new Message<string, T>
        {
            Key = key,
            Value = value
        };
        
        var deliveryReport = await _producer.ProduceAsync(topicName, message, cancellationTokenSource.Token);
    }
}
