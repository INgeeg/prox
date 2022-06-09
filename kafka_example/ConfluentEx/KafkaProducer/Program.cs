using AvroSpecific;
using KafkaProducer.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        IHostEnvironment environment;
        var topic = "";
        var host = builder.ConfigureServices(services =>
        {   
            if(args.Length > 0 && args[0] == "dev")// (environment.IsDevelopment())
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
        var producer = factory.CreateProducer<User>();
        await producer.ProduceAsync(topic,"key", new User(){name = "user1", 
            favorite_color = "alik", 
            favorite_number = 123, 
            hourly_rate = new Avro.AvroDecimal(67.99)  }, cancellationTokenSource);


        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        // CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        // IHostBuilder builder = Host.CreateDefaultBuilder(args).ConfigureLogging(logging =>
        // {
        //     logging.ClearProviders();
        //     logging.AddConsole();
        // });
        //
        // IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        // IHostEnvironment environment;
        // var host = builder.ConfigureServices(services =>
        // {   
        //     if(args.Length > 0 && args[0] == "dev")// (environment.IsDevelopment())
        //     {
        //         services.AddConfluentKafkaProducer(configuration);
        //     }
        //     else
        //     {
        //         services.AddAzureKafkaProducer(configuration);
        //     }
        // }).Build();
        //
        // IEventProducerFactory factory = host.Services.GetRequiredService<IEventProducerFactory>();
        // var producer = factory.CreateProducer<User>();
        // await producer.ProduceAsync("key", new User(){name = "user1", 
        //     favorite_color = "alik", 
        //     favorite_number = 123, 
        //     hourly_rate = new Avro.AvroDecimal(67.99)  }, cancellationTokenSource);
        //
        //
        
        
        
        
        /////////
        /// ///
        /// //
        /// ////
        
        

        // IEventProducer<User> producer;
        //
        // if (args.Length > 0 && args[0] == "dev")
        // {
        //     
        //     var host = builder.ConfigureServices(services => {services.Configure<ConfulentKafkaConfiguration>(configuration.GetSection("Confluent:Producer"));}).Build();
        //     var options = host.Services.GetRequiredService<IOptions<ConfulentKafkaConfiguration>>();
        //     var logger = host.Services.GetRequiredService<ILogger<ConfluentKafkaProducer<User>>>();
        //     producer = new ConfluentKafkaProducer<User>(options, ConfluentKafkaEventProducerFactory.BuildConfluentProducer<User>(options),logger);
        // }
        // else
        // {
        //     var host = builder.ConfigureServices(services => {services.Configure<AzureKafkaConfiguration>(configuration.GetSection("EventHub:Producer")); }).Build();
        //     var options = host.Services.GetRequiredService<IOptions<AzureKafkaConfiguration>>();
        //     var logger = host.Services.GetRequiredService<ILogger<AzureKafkaProducer<User>>>();
        //     producer = new AzureKafkaProducer<User>(options, ConfluentKafkaEventProducerFactory.BuildAzureProducer<User>(options),logger);
        //     
        // }
        //
        // await producer.ProduceAsync("key",
        //     new User(){name = "user1", 
        //         favorite_color = "alik", 
        //         favorite_number = 123, 
        //         hourly_rate = new Avro.AvroDecimal(67.99) 
        //     }, cancellationTokenSource);
    }
    
}
