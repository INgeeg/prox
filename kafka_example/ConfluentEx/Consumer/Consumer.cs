using Confluent.Kafka;
using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using AvroSpecific;
using Azure.Data.SchemaRegistry;
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Azure.Data.SchemaRegistry.ApacheAvro;
using Microsoft.Extensions.Azure;

class Consumer {

    static void Main(string[] args)
    {
        KafkaConsumer.Run().Wait();
        //ConsumeAzureCloudWithSchema_kafka(args);
        //ConsumeConfluentCloudWithAzureSchema_kafka(args);
        ConsumeMessageToAzureEventHubWithAvro(args).Wait();
        //ConsumeMessageToConfluentCloudCluster(args);
    }

    static void ConsumeAzureCloudWithSchema_kafka(string[] args)
    {
        
       
    }

    static void ConsumeConfluentCloudWithAzureSchema_kafka(string[] args)
    {
        string bootstrapServers = "ddddst4.gcp.confluent.cloud:9092";
        string schemaRegistryUrl = "https://ddddd.westus2.azure.confluent.cloud";
        string topicName = "test_topic_scm";
        
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "dddd",
            SaslPassword = "4dopyfjZIOzp3SSVA",
            GroupId = "group1",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        
   
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = schemaRegistryUrl,
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo, 
            BasicAuthUserInfo = "OdCxz82iminAsQ9MHH0a3ArI4NmPjHrnU2QbVeb6/vgG2S30WN60"
        };

        CancellationTokenSource cts = new CancellationTokenSource();
        // var consumeTask = Task.Run(() =>
        // {
            using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
            using (var consumer =
                   new ConsumerBuilder<string, User>(consumerConfig)
                       .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                       .SetValueDeserializer(new AvroDeserializer<User>(schemaRegistry).AsSyncOverAsync())
                       .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                       .Build())
            {
                consumer.Subscribe(topicName);

                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cts.Token);
                            Console.WriteLine($"user name: {consumeResult.Message.Key}, favorite color: {consumeResult.Message.Value.favorite_color}, hourly_rate: {consumeResult.Message.Value.hourly_rate}");
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Consume error: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
            }
            
        // });
        cts.Cancel();
    }
    static async Task ConsumeMessageToAzureEventHubWithAvro(string[] args)
    {
        
        IConfiguration configuration = new ConfigurationBuilder()
            .AddIniFile(args[0])
            .Build();
        
        string tenantId = configuration.GetValue<string>("azure.tenantid");
        string clientId = configuration.GetValue<string>("azure.clientid");
        string clientSecret = configuration.GetValue<string>("azure.clientsecret");
        string schemaName = configuration.GetValue<string>("eventhub.schema");
        string connectionString = configuration.GetValue<string>("eventhub.connectionstring");
        string endpoint = configuration.GetValue<string>("eventhub.endpoint");
        var consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;//"customgroupname";
        
        var credentials = new ClientSecretCredential(tenantId,clientId,clientSecret);
        var client = new SchemaRegistryClient(endpoint, credentials);
        var serializer = new SchemaRegistryAvroSerializer(client, schemaName, new SchemaRegistryAvroSerializerOptions { AutoRegisterSchemas = true });

        //await using var consumer = new EventHubConsumerClient(consumerGroup, fullyQualifiedNamespace, eventHubName, credential);
        await using var consumer = new EventHubConsumerClient(consumerGroup,connectionString,"my_topic");
        await foreach (PartitionEvent receivedEvent in consumer.ReadEventsAsync())
        {
            if (receivedEvent.Data.ContentType != null)
            {
                Employee deserialized =
                    (Employee) await serializer.DeserializeAsync(receivedEvent.Data, typeof(Employee),
                        CancellationToken.None);
                Console.WriteLine(deserialized.Age);
                Console.WriteLine(deserialized.Name);
            }
            //break;
        }
    }
    static void ConsumeMessageToConfluentCloudCluster(string[] args)
    {
        if (args.Length != 1) {
            Console.WriteLine("Please provide the configuration file path as a command line argument");
        }

        IConfiguration configuration = new ConfigurationBuilder()
            .AddIniFile(args[0])
            .Build();
        //configuration["auto.offset.reset"] = "earliest";

        const string topic = "test_topic";

        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true; // prevent the process from terminating.
            cts.Cancel();
        };

        using (var consumer = new ConsumerBuilder<string, string>(
            configuration.AsEnumerable()).Build())
        {
            consumer.Subscribe(topic);
            try {
                while (true) {
                    var cr = consumer.Consume(cts.Token);
                    Console.WriteLine($"Consumed event from topic {topic} with key {cr.Message.Key,-10} and value {cr.Message.Value}");
                }
            }
            catch (OperationCanceledException) {
                // Ctrl-C was pressed.
            }
            finally{
                consumer.Close();
            }
        }
    }
}