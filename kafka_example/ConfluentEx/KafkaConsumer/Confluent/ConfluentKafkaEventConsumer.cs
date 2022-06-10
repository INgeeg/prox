using Confluent.Kafka;

namespace KafkaConsumer.Confluent;

public class ConfluentKafkaEventConsumer<TValue>: IEventConsumer<TValue>
{
    private readonly IConsumer<string, TValue> _consumer;

    public ConfluentKafkaEventConsumer(IConsumer<string, TValue> consumer)
    {
        _consumer = consumer;
    }

    public async IAsyncEnumerable<TValue> ConsumeAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var msg = _consumer.Consume(cancellationToken);
            yield return (TValue) Convert.ChangeType(msg.Message.Value, typeof(TValue));
        }
    }

    public void Dispose()
    {
        _consumer.Dispose();
    }

    public void Subscribe(string topic)
    {
        _consumer.Subscribe(topic);
    }

}