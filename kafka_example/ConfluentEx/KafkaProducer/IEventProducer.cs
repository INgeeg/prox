namespace KafkaProducer;
public interface IEventProducer<TKey, TValue>: IDisposable
{
    public Task ProduceAsync(string topic, TKey key, TValue message, CancellationTokenSource cancellationTokenSource);
    void Flush(CancellationToken cancellationToken = default(CancellationToken));
}