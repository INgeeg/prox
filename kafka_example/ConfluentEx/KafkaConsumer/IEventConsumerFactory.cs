namespace KafkaConsumer;

public interface IEventConsumerFactory
{
    IEventConsumer<TValue> CreateEventConsumer<TValue>();
}
