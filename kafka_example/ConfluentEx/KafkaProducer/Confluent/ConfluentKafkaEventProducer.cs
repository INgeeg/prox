using Confluent.Kafka;


namespace KafkaProducer.Confluent;
public class ConfluentKafkaEventProducer<TKey, TValue> : IEventProducer<TKey, TValue>
{
    private readonly IProducer<TKey, TValue> _producer;

    public ConfluentKafkaEventProducer(IProducer<TKey, TValue> producer)
    {
        _producer = producer;
    }
    
    public void Dispose()
    {
        _producer.Dispose();
    }

    public void Flush(CancellationToken cancellationToken = default)
    {
        _producer.Flush(cancellationToken);
    }
    public async Task ProduceAsync(string topic, TKey key, TValue value, CancellationTokenSource cancellationTokenSource)
    {
        var message = new Message<TKey, TValue>
        {
            Key = key, 
            Value = value
        };
        var deliveryReport = await _producer.ProduceAsync(topic, message,cancellationTokenSource.Token);
    }
}
