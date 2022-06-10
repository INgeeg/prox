
using Confluent.Kafka;

namespace KafkaProducer.Azure;

public class AzureKafkaEventProducer<TKey, TValue> : IEventProducer<TKey,TValue>
{
    private readonly IProducer<TKey, TValue> _producer;

    public AzureKafkaEventProducer(IProducer<TKey, TValue> producer)
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
    public async Task ProduceAsync(string topicName, TKey key, TValue value, CancellationTokenSource cancellationTokenSource)
    {
        var message = new Message<TKey, TValue>
        {
            Key = key,
            Value = value
        };
        var deliveryReport = await _producer.ProduceAsync(topicName, message, cancellationTokenSource.Token);
    }
}