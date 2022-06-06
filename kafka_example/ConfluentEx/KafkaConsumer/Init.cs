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
            services.AddTransient<IEventConsumer,AzureKafkaConsumer>();
            //services.AddTransient<IEventConsumer,KafkaConsumer>();
        }).Build();

        var consumer = host.Services.GetRequiredService<IEventConsumer>();
        //confluent topic:  test_topic_scm ---------   azure topic: my_topic
        await foreach (var user in consumer.ConsumeAsync<User>("my_topic", "group1"))
        {
            Console.WriteLine(user.favorite_color);
        }
    }
}