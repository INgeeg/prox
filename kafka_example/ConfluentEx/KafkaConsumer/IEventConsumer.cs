namespace KafkaConsumer;
public interface IEventConsumer<TValue>: IDisposable
{
    public void Subscribe(string topic);
    public IAsyncEnumerable<TValue> ConsumeAsync(CancellationToken cancellationToken);
}