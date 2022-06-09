public interface IEventProducer<T>
{
    public Task ProduceAsync(string topic, string key, T message, CancellationTokenSource cancellationTokenSource);
}