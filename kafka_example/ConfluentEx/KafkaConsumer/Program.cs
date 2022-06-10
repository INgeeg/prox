using AvroSpecific;
using KafkaConsumer.Azure;
using KafkaConsumer.Confluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaConsumer;
public class Program
{
    static async Task Main(string[] args)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        IHostBuilder builder = Host.CreateDefaultBuilder(args).ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        });
        
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();
        var topic = "";
        var host = builder.ConfigureServices(services =>
        {   
            if(args.Length > 0 && args[0] == "dev")
            {
                services.AddConfluentKafkaEventConsumer(configuration);
                topic = "test_topic_scm";
            }
            else
            {
                services.AddAzureKafkaEventConsumer(configuration);
                topic = "my_topic";
            }
        }).Build();
        
        var factory = host.Services.GetRequiredService<IEventConsumerFactory>();

        using (var consumer = factory.CreateEventConsumer<User>())
        {
            consumer.Subscribe(topic);

            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            await foreach (var user in consumer.ConsumeAsync(cancellationTokenSource.Token))
            {
                Console.WriteLine($"Message received: User: {{name: {user.name}, favorite_number: {user.favorite_number}}} from topic: {topic}");
            }
        }
    }
    
}