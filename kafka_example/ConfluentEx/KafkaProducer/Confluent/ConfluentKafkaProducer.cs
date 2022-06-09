using Confluent.Kafka;

public class ConfluentKafkaProducer<T> : IEventProducer<T>
{
    private readonly IProducer<string, T> _producer;

    public ConfluentKafkaProducer(IProducer<string,T> producer)
    {
        _producer = producer;
    }
    public async Task ProduceAsync(string topic, string key, T value, CancellationTokenSource cancellationTokenSource)
    {
        var message = new Message<string, T>
        {
            Key = key, 
            Value = value
        };
        var deliveryReport = await _producer.ProduceAsync(topic, message,cancellationTokenSource.Token);
    }
}
