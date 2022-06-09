
using Confluent.Kafka;
using Microsoft.Extensions.Options;

public class ConfluentKafkaConsumer<T>: IEventConsumer<T>
{
    private readonly IOptions<ConfluentKafkaConfiguration> _options;
    private readonly IConsumer<string, T> _consumer;
    private readonly ILoggerAdapter<ConfluentKafkaConsumer<T>> _logger;

    public ConfluentKafkaConsumer(IOptions<ConfluentKafkaConfiguration> options,IConsumer<string, T> consumer, ILoggerAdapter<ConfluentKafkaConsumer<T>> logger)
    {
        _options = options;
        _consumer = consumer;
        _logger = logger;
    }
    public async IAsyncEnumerable<T> ConsumeAsync(CancellationTokenSource cancellationTokenSource)
    {
        _consumer.Subscribe(_options.Value.TopicName);
        _logger.LogInformation("Consuming messages from topic: " + _options.Value.TopicName + ", broker(s): " + _options.Value.Endpoint);
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            var msg = _consumer.Consume(cancellationTokenSource.Token);
            yield return (T) Convert.ChangeType(msg.Message.Value, typeof(T));
        }
        
    }
}
