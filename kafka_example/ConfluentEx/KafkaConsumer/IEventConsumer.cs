
using Avro.Specific;
using Confluent.Kafka;

public interface IEventConsumer
{
    public IAsyncEnumerable<T> ConsumeAsync<T>(string topic, string group);

}