using AvroSpecific;
using KafkaProducer.Azure;
using KafkaProducer.Confluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaProducer;
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
                services.AddConfluentKafkaProducer(configuration);
                topic = "test_topic_scm";
            }
            else
            {
                services.AddAzureKafkaProducer(configuration);
                topic = "my_topic";
            }
        }).Build();
        
        IEventProducerFactory factory = host.Services.GetRequiredService<IEventProducerFactory>();

        using (var producer = factory.CreateEventProducer<string?, User>())
        {
            var user = new User() {name = "John", favorite_color = "green", favorite_number = new Random().Next(), hourly_rate = new Avro.AvroDecimal(67.99)};
            await producer.ProduceAsync(topic, null,user,cancellationTokenSource);
            
            producer.Flush(cancellationTokenSource.Token);
            Console.WriteLine($"Message User: {{name: {user.name}, favorite_number: {user.favorite_number}}} sent to topic: {topic}");
        }

    }
    
}
