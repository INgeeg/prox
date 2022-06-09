using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


public static class ConfluentKafkaServiceCollectionExtensions
{ 
    public static IServiceCollection AddConfluentKafkaProducer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConfluentKafkaConfiguration>(configuration.GetSection("Confluent:Producer"));
        services.AddSingleton<IEventProducerFactory>(provider => 
            new ConfluentKafkaEventProducerFactory(
                provider.GetRequiredService<IOptions<ConfluentKafkaConfiguration>>(),
                provider.GetRequiredService<ILogger<ConfluentKafkaEventProducerFactory>>()
            )
        );
        
        return services;
    }

}