
public interface IEventProducerFactory
{
    IEventProducer<T> CreateProducer<T>();
}