using Confluent.Kafka;

namespace KafkaConsumer.Azure;

public class AzureKafkaEventConsumer<T>: IEventConsumer<T>
{
    private readonly IConsumer<string, T> _consumer;

    public AzureKafkaEventConsumer(IConsumer<string, T> consumer)
    {
        _consumer = consumer;
    }

    public async IAsyncEnumerable<T> ConsumeAsync(CancellationToken cancelationToken)
    {
        
        while (!cancelationToken.IsCancellationRequested)
        {
            var msg = _consumer.Consume(cancelationToken);
            yield return (T) Convert.ChangeType(msg.Message.Value, typeof(T));
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