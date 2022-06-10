namespace KafkaProducer;
public interface IEventProducerFactory
{
    IEventProducer<TKey,TValue> CreateEventProducer<TKey,TValue>();
}