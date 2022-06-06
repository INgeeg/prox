
using AvroSpecific;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Init
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);

        var host = builder.ConfigureServices(services =>
        {
            services.AddTransient<IEventProducer,AzureKafkaProducer>();
            //services.AddTransient<IEventProducer,KafkaProducer>();
        }).Build();

        var consumer = host.Services.GetRequiredService<IEventProducer>();
        //confluent topic:  test_topic_scm ---------   azure topic: my_topic
        await consumer.ProduceAsync<User>("my_topic","key",
            new User(){name = "user1", 
                favorite_color = "alik", 
                favorite_number = 123, 
                hourly_rate = new Avro.AvroDecimal(67.99) 
            });
    }
}