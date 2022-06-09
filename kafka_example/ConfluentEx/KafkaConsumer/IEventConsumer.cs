public interface IEventConsumer<T>
{
    public IAsyncEnumerable<T> ConsumeAsync(CancellationTokenSource cancellationTokenSource);

}