using System.Threading;
using System.Threading.Tasks;
using AvroSpecific;
using Confluent.Kafka;
using KafkaProducer.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;


public class AzureKafkaProducerTests
{
    
    private IOptions<AzureSchemaRegistryConfiguration> _options;
    private readonly Mock<CancellationTokenSource> _mockCts;
    private readonly Mock<IProducer<string, User>> _mockProducer;
    private readonly Mock<ILogger<AzureKafkaEventProducer<,>>> _logger;
    private readonly AzureKafkaEventProducer<,> _sut;
    
    public AzureKafkaProducerTests()
    {
        _options = Options.Create(new AzureSchemaRegistryConfiguration());
        _mockCts = new Mock<CancellationTokenSource>();
        _mockProducer = new Mock<IProducer<string, User>>();
        _logger = new Mock<ILogger<AzureKafkaEventProducer<,>>>();
        _sut = new AzureKafkaEventProducer<,>(_options, _mockProducer.Object,_logger.Object);
    }
    
    [Fact]
    public async Task ProduceAsync_ShouldProduceToSameTopic_WhenSendingViaOptions()
    {
       
        _options.Value.TopicName = "fakeTopic";
        await _sut.ProduceAsync("fakeKey", new User(), _mockCts.Object);
        
        _mockProducer.Verify(x=>x.ProduceAsync(
            It.Is<string>(x=>x==_options.Value.TopicName),
            It.IsAny<Message<string,User>>(),
            It.IsAny<CancellationToken>()),Times.Once);
    }
    
    [Fact]
    public async Task ProduceAsync_ShouldProduceSameKeyAndValue_WhenPassingKeyAndValue()
    {
        var user = new User() { favorite_color = "fakeValue"};
        var fakeKey = "fakeKey";
        
        await _sut.ProduceAsync(fakeKey, user, _mockCts.Object);
        
        _mockProducer.Verify(x=>x.ProduceAsync(
            It.IsAny<string>(),
            It.Is<Message<string,User>>(x =>
                x.Value.favorite_color == user.favorite_color 
                && x.Key==fakeKey),
            It.IsAny<CancellationToken>()),Times.Once);
    }
    
    [Fact]
    public async Task ProduceAsync_ShouldLogMessages_WhenMethodExecuted()
    {
        var fakeKey = "fakeKey";
        var fakeUser = new User();
        _options.Value.TopicName = "fakeTopic";
        _options.Value.Endpoint = "fakeEndpoint";
        var firstMessage = "Sending 1 message to topic: " + _options.Value.TopicName + ", broker(s): " + _options.Value.Endpoint;
        var secondMessage = string.Format("Message {0} sent (value: '{1}')", fakeKey, fakeUser);
        
        await _sut.ProduceAsync(fakeKey, fakeUser, _mockCts.Object);
        
        _logger.Verify(x=>
            x.LogInformation(
                It.Is<string>(x=>x==firstMessage),
                It.IsAny<string[][]>()),Times.Once);
        
        _logger.Verify(x=>
            x.LogInformation(
                It.Is<string>(x=>x==secondMessage),
                It.IsAny<string[][]>()),Times.Once);
    }
}