using AvroSpecific;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

public class Program
{
    static async Task Main(string[] args)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        IHostBuilder builder = Host.CreateDefaultBuilder(args);
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        
        IEventConsumer<User> consumer;

        if (args.Length > 0 && args[0] == "azure")
        {
            var host = builder.ConfigureServices(services => {services.Configure<AzureKafkaConfiguration>(configuration.GetSection("EventHub:Consumer"));}).Build();
            var options = host.Services.GetRequiredService<IOptions<AzureKafkaConfiguration>>();
            var logger = host.Services.GetRequiredService<ILoggerAdapter<AzureKafkaConsumer<User>>>();
            consumer = new AzureKafkaConsumer<User>(options, BuildAzureConsumer<User>(options),logger);
        }
        else
        {
            var host = builder.ConfigureServices(services => {services.Configure<ConfluentKafkaConfiguration>(configuration.GetSection("Confluent:Consumer"));}).Build();
            var options = host.Services.GetRequiredService<IOptions<ConfluentKafkaConfiguration>>();
            var logger = host.Services.GetRequiredService<ILoggerAdapter<ConfluentKafkaConsumer<User>>>();
            consumer = new ConfluentKafkaConsumer<User>(options, BuildConfluentConsumer<User>(options),logger);
        }
        
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cancellationTokenSource.Cancel();
        };
        
        await foreach (var user in consumer.ConsumeAsync(cancellationTokenSource).WithCancellation(cancellationTokenSource.Token))
        {
            Console.WriteLine(user.favorite_color);
        }
    }
    
    static IConsumer<string,T> BuildAzureConsumer<T>(IOptions<AzureKafkaConfiguration> options)
    {
        var consumerConfig = new ConsumerConfig()
        {
            BootstrapServers = options.Value.Endpoint,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = options.Value.SaslUsername,
            SaslPassword = options.Value.SaslPassword,
            GroupId = options.Value.GroupId,
            AutoOffsetReset = options.Value.Offset
        };
        
        IAsyncDeserializer<T> deserializer = new AzureAvroDeserializer<T>(options.Value.SchemaRegistryUrl,
            options.Value.AzureSchemaGroup, options.Value.AzureTenantId, options.Value.AzureClientId,
            options.Value.AzureClientSecret);
   
        return new ConsumerBuilder<string, T>(consumerConfig)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(deserializer.AsSyncOverAsync())
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .Build();
    }
    static IConsumer<string,T> BuildConfluentConsumer<T>(IOptions<ConfluentKafkaConfiguration> options)
    {
        var consumerConfig = new ConsumerConfig()
        {
            BootstrapServers = options.Value.Endpoint,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = options.Value.SaslUsername,
            SaslPassword = options.Value.SaslPassword,
            GroupId = options.Value.GroupId,
            AutoOffsetReset = options.Value.Offset
        };
        
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = options.Value.SchemaRegistryUrl,
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo, 
            BasicAuthUserInfo = options.Value.SchemaRegistryUrlBasicAuth
        };
        var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        return new ConsumerBuilder<string, T>(consumerConfig)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(new AvroDeserializer<T>(schemaRegistry).AsSyncOverAsync())
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .Build();
    }
}