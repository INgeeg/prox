//using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AvroSpecific;
using Azure.Core;
using Azure.Data.SchemaRegistry;
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using Confluent.Kafka.Admin;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Azure.Data.SchemaRegistry.ApacheAvro;
using Microsoft.Extensions.Configuration;


class Producer {

    static void Main(string[] args)
    {
        //AzureProducer.Run().Wait();
        
        KafkaProducer.Run().Wait();
        //ProduceAzureCloudWithSchema_kafka(args).Wait();
        //ProduceConfluentCloudWithSchema_kafka(args).Wait();
        //ProduceMessageToAzureEventHubWithAvro(args).Wait();
        //ProduceMessageToConfluentCloudCluster(args);
    }

    static async Task ProduceAzureCloudWithSchema_kafka(string[] args)
    {
        string brokerList = "dddd.servicebus.windows.net:9093";
        string connectionString = "Endpoint=sb://ddd.servicebus.windows.net/;SharedAccessKeyName=mypolicy;SharedAccessKey=Tx378p9b3V/sdfd=;EntityPath=asd";
        string topic = "test_topic_scm";
        string caCertLocation = "";
        string consumerGroup = "$Default";
        string schemaRegistryUrl = "ddd.servicebus.windows.net";
        string schemaGroup = "AvroSchm";
        /////////////////
   

        string[] users = { "eabara", "jsmith", "sgarcia", "jbernard", "htanaka", "awalther" };
        string[] items = { "book", "alarm clock", "t-shirts", "gift card", "batteries" };
        
        var config = new ProducerConfig
        {
            BootstrapServers = brokerList,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = "$ConnectionString",
            SaslPassword = connectionString,
            SslCaLocation = ""
            //,Debug = "security,broker,protocol"        //Uncomment for librdkafka debugging information
        };
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = schemaRegistryUrl,
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo, 
            BasicAuthUserInfo = "7bcbe23d5b:nNa7Q~kRk_dIKnCkqBTM"
        };
        using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
        using (var producer = new ProducerBuilder<string, User>(config)
        //using (var producer = new ProducerBuilder<string, string>(config)
                   .SetKeySerializer(Serializers.Utf8)
                   .SetValueSerializer(new AvroSerializer<User>(schemaRegistry))
                   .Build())
        {
            var numProduced = 0;
            const int numMessages = 10;
            for (int i = 0; i < numMessages; ++i)
            {
                Random rnd = new Random();
                var user = users[rnd.Next(users.Length)];
                var item = items[rnd.Next(items.Length)];
                User item2 = new User { name = user, favorite_color = "green", favorite_number = i++, hourly_rate = new Avro.AvroDecimal(67.99) };
                var deliveryReport = 
                    await producer.ProduceAsync(topic, new Message<string, User> { Key = user, Value = item2 });
                    //await producer.ProduceAsync(topic, new Message<string, string> { Key = user, Value = item });
                   
                
                 // producer.Produce(topic, new Message<string, string> { Key = user, Value = item },
                 //     (deliveryReport) =>
                 //     {
                 //         if (deliveryReport.Error.Code != ErrorCode.NoError) {
                 //             Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                 //         }
                 //         else {
                 //             Console.WriteLine($"Produced event to topic {topic}: key = {user,-10} value = {item}");
                 //             numProduced += 1;
                 //         }
                 //     });
            }

            producer.Flush(TimeSpan.FromSeconds(10));
            Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
        }
        
    }

    static async Task ProduceConfluentCloudWithSchema_kafka(string[] args)
    {
        string bootstrapServers = "ddd.us-west4.gcp.confluent.cloud:9092";
        string schemaRegistryUrl = "https://ddd.westus2.azure.confluent.cloud";
        string topicName = "test_topic_scm";
        
        var producerConfig = new ProducerConfig()
        {
            BootstrapServers = bootstrapServers,
            SaslMechanism = SaslMechanism.Plain,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslUsername = "dddd",
            SaslPassword = "4uw4Qhddd"
        };
   
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = schemaRegistryUrl,
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo, 
            BasicAuthUserInfo = "ddd4NmPjHrnU2QbVeb6/vgG2S30WN60"
        };

        var avroSerializerConfig = new AvroSerializerConfig
        {
            BufferBytes = 100
        };

        CancellationTokenSource cts = new CancellationTokenSource();
            

        using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
        using (var producer =
            new ProducerBuilder<string, User>(producerConfig)
                 .SetKeySerializer(new AvroSerializer<string>(schemaRegistry, avroSerializerConfig))
                .SetValueSerializer(new AvroSerializer<User>(schemaRegistry, avroSerializerConfig))
                .Build())
        {
            Console.WriteLine($"{producer.Name} producing on {topicName}. Enter user names, q to exit.");

            int i = 0;
            string text;
            while ((text = Console.ReadLine()) != "q")
            {
                User user = new User { name = text, favorite_color = "green", favorite_number = i++, hourly_rate = new Avro.AvroDecimal(67.99) };
                await producer
                    .ProduceAsync(topicName, new Message<string, User> { Key = text, Value = user })
                    .ContinueWith(task =>
                        {
                            if (!task.IsFaulted)
                            {
                                Console.WriteLine($"produced to: {task.Result.TopicPartitionOffset}");
                            }

                            // Task.Exception is of type AggregateException. Use the InnerException property
                            // to get the underlying ProduceException. In some cases (notably Schema Registry
                            // connectivity issues), the InnerException of the ProduceException will contain
                            // additional information pertaining to the root cause of the problem. Note: this
                            // information is automatically included in the output of the ToString() method of
                            // the ProduceException which is called implicitly in the below.
                            //Console.WriteLine($"error producing message: {task.Exception.InnerException}");
                        });
            }
        }

        cts.Cancel();
    }

    static async Task ProduceMessageToAzureEventHubWithAvro(string[] args)
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
        
        var credentials = new ClientSecretCredential(tenantId,clientId,clientSecret);
        var client = new SchemaRegistryClient(endpoint, credentials);
        var serializer = new SchemaRegistryAvroSerializer(client, schemaName, new SchemaRegistryAvroSerializerOptions { AutoRegisterSchemas = true });
        
        var dataEvents = new List<EventData>();
        
        for (int i = 0; i < 2; i++)
        {
            var employee = new Employee { Age = i+20, Name = "Caketown" };
            EventData eventData = (EventData) await serializer.SerializeAsync(employee, messageType: typeof(EventData));
            dataEvents.Add(eventData);
            Console.WriteLine($"Schema id: {eventData.ContentType}      Data: {eventData.EventBody}");
        }
        
        // the schema Id will be included as a parameter of the content type
        //Console.WriteLine(eventData.ContentType);
        
        // the serialized Avro data will be stored in the EventBody
        //Console.WriteLine(eventData.EventBody);
 
        //await using var producer = new EventHubProducerClient(fullyQualifiedNamespace, eventHubName, credential);
        await using var producer = new EventHubProducerClient(connectionString);
        await producer.SendAsync(dataEvents);
        
    }


    //can be used for AZURE Event Hub also by just changing the properties file
    static void ProduceMessageToConfluentCloudCluster(string[] args)
    {
        if (args.Length != 1) {
            Console.WriteLine("Please provide the configuration file path as a command line argument");
        }

        IConfiguration configuration = new ConfigurationBuilder()
            .AddIniFile(args[0])
            .Build();

        string topic = "test_topic_scm";

        string[] users = { "eabara", "jsmith", "sgarcia", "jbernard", "htanaka", "awalther" };
        string[] items = { "book", "alarm clock", "t-shirts", "gift card", "batteries" };

        using (var producer = new ProducerBuilder<string, string>(
            configuration.AsEnumerable()).Build())
        {
            var numProduced = 0;
            const int numMessages = 10;
            for (int i = 0; i < numMessages; ++i)
            {
                Random rnd = new Random();
                var user = users[rnd.Next(users.Length)];
                var item = items[rnd.Next(items.Length)];

                producer.Produce(topic, new Message<string, string> { Key = user, Value = item },
                    (deliveryReport) =>
                    {
                        if (deliveryReport.Error.Code != ErrorCode.NoError) {
                            Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                        }
                        else {
                            Console.WriteLine($"Produced event to topic {topic}: key = {user,-10} value = {item}");
                            numProduced += 1;
                        }
                    });
            }

            producer.Flush(TimeSpan.FromSeconds(10));
            Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
        }
    }
}