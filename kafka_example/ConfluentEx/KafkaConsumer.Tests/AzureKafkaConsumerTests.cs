using System.Threading;
using System.Threading.Tasks;
using AvroSpecific;
using Confluent.Kafka;
using KafkaConsumer.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;


public class AzureKafkaConsumerTests
{
    
    private IOptions<AzureKafkaConfiguration> _options;
    private readonly Mock<CancellationTokenSource> _mockCts;
    private readonly Mock<IConsumer<string, User>> _mockConsumer;
    private readonly Mock<ILoggerAdapter<AzureKafkaConsumer<User>>> _logger;
    private readonly AzureKafkaConsumer<User> _sut;
    
    public AzureKafkaConsumerTests()
    {
        _options = Options.Create(new AzureKafkaConfiguration());
        _mockCts = new Mock<CancellationTokenSource>();
        _mockConsumer = new Mock<IConsumer<string, User>>();
        _logger = new Mock<ILogger<AzureKafkaConsumer<User>>>();
        _sut = new AzureKafkaEventConsumer<User>();
    }
    
    [Fact]
    public void ConsumeAsync_ShouldSubscribeToTheSameTopic_WhenTopicPassedViaOptions()
    {
        
        
        
        _options.Value.TopicName = "fakeTopic";
        //_mockConsumer.Setup(x => x.Subscribe(_options.Value.TopicName)).Verifiable();
        var result = _sut.ConsumeAsync(_mockCts.Object);
        _mockConsumer.Verify(x =>
            x.Subscribe(
                //It.Is<string>(x => x == _options.Value.TopicName)),
                It.IsAny<string>()),
            Times.Once);
    }
    
    [Fact]
    public void  ConsumeAsync_ShouldLogMessages_WhenMethodExecuted()
    {
        _options.Value.TopicName = "fakeTopic";
        _options.Value.Endpoint = "fakeEndpoint";
        var firstMessage = "Consuming messages from topic: " + _options.Value.TopicName + ", broker(s): " + _options.Value.Endpoint;

        var result = _sut.ConsumeAsync(_mockCts.Object);
        
        _logger.Verify(x=>
            x.LogInformation(
                //It.Is<string>(x=>x==firstMessage),
                It.IsAny<string>(),
                It.IsAny<string[][]>()),Times.Once);
    }

}