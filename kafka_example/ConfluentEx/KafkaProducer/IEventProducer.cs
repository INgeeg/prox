

public interface IEventProducer
{
    public Task ProduceAsync<T>(string topic, string key, T message);
}