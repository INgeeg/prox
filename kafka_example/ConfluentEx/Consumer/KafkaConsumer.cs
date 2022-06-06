using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avro.Specific;
using AvroSpecific;
using Azure.Core;
using Azure.Data.SchemaRegistry;
using Azure.Identity;
using Azure.Messaging;
using Azure.Messaging.EventHubs;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Azure.Data.SchemaRegistry.ApacheAvro;
using Microsoft.Identity.Client;

public class KafkaConsumer
{
    public static async Task Run()
    {
        string brokerList = "ddd.servicebus.windows.net:9093";
        string connectionString = "Endpoint=sb://ddd.servicebus.windows.net/;SharedAccessKeyName=mypolicy;SharedAccessKey=dedNgOyjMs92zq1kQudwk=;EntityPath=d";
        string topic = "my_topic";
        string caCertLocation = "";
        string consumerGroup = "$Default";string schemaRegistryUrl = "ddd.servicebus.windows.net";
        string schemaGroup = "AvroSchm";
        
        var config = new ConsumerConfig
        {
            BootstrapServers = brokerList,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SocketTimeoutMs = 60000,                //this corresponds to the Consumer config `request.timeout.ms`
            SessionTimeoutMs = 30000,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = "$ConnectionString",
            SaslPassword = connectionString,
            //SslCaLocation = cacertlocation,
            GroupId = consumerGroup,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            BrokerVersionFallback = "1.0.0",        //Event Hubs for Kafka Ecosystems supports Kafka v1.0+, a fallback to an older API will fail
            //Debug = "security,broker,protocol"    //Uncomment for librdkafka debugging information
        };
        
        var consumerConfig = new ConsumerConfig()
        {
            BootstrapServers = brokerList,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = "$ConnectionString",
            SaslPassword = connectionString,
            GroupId = "group1",
            AutoOffsetReset = AutoOffsetReset.Earliest 
            //SslCaLocation = cacertlocation,
            //Debug = "security,broker,protocol"        //Uncomment for librdkafka debugging information
        };
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = "https://ddddd.westus2.azure.confluent.cloud",
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo, 
            BasicAuthUserInfo = "OTddArI4NmPjHrnU2dvgG2S30WN60"
        };
        
        var credentials = new ClientSecretCredential("6fd60f2fdf1720f","7bcbe2da5b","nNa7Q~kRk_CYkjxMdTM");
        var valueDeserializer = new KafkaAvroDeserializer<User>(schemaRegistryUrl, credentials, schemaGroup);

        
        
        //using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
        // using (var consumer = new ConsumerBuilder<long, string>(config)
        //            .SetKeyDeserializer(Deserializers.Int64)
        //            .SetValueDeserializer(Deserializers.Utf8)
        using (var consumer = new ConsumerBuilder<string, User>(consumerConfig)
                   .SetKeyDeserializer(Deserializers.Utf8)
                   //.SetValueDeserializer(new AvroDeserializer<User>(schemaRegistry).AsSyncOverAsync())
                   .SetValueDeserializer(valueDeserializer.AsSyncOverAsync())
                   //.SetValueDeserializer(Deserializers.ByteArray)
                   .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                   .Build())
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            consumer.Subscribe(topic);

            Console.WriteLine("Consuming messages from topic: " + topic + ", broker(s): " + brokerList);

            while (true)
            {
                try
                {
                    var msg = consumer.Consume(cts.Token);
                    Console.WriteLine($"Received: {msg.Message.Value}");
                    //Console.WriteLine($"output: {String.Join("--",msg.Message.Value)}");
                    Console.WriteLine($"Received: {{name:{msg.Value.name} favorite_number:{msg.Value.favorite_number}}}");
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consume error: {e.Error.Reason}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                    break;
                }
            }
        }
    }
}

public class KafkaAvroDeserializer<T> :IAsyncDeserializer<T>
{
    private readonly SchemaRegistryAvroSerializer serializer;
    //private readonly SchemaRegistryClient _client;

    /// <summary>
    /// Constructor for KafkaAvroDeserializer.
    /// </summary>
    /// <param name="schemaRegistryUrl"></param> URL endpoint for Azure Schema Registry instance
    /// <param name="credential"></param> TokenCredential implementation for OAuth2 authentication
    /// <param name="schemaGroup"></param> SchemaGroup implementation 
    public KafkaAvroDeserializer(string schemaRegistryUrl, TokenCredential credential,string schemaGroup="$default")
    {
        this.serializer = new SchemaRegistryAvroSerializer(new SchemaRegistryClient(schemaRegistryUrl, credential), schemaGroup);
        //this._client = new SchemaRegistryClient(schemaRegistryUrl, credential);
    }

    public Task<T> DeserializeAsync(ReadOnlyMemory<byte> data, 
        bool isNull, SerializationContext context)
    {
        if (!data.IsEmpty)
        {
        }

        // var user = new User();
        // Avro.Schema schema = user.Schema;
        // string schemaString = schema.ToString();
        byte[] result = null;
        var schemeid = context.Headers.TryGetLastBytes("schemaId",out result);
        //var schema2 = _client.GetSchema(result.ToString());
        //SchemaProperties schemaProperties = _client.GetSchemaProperties("AvroSchm", schema.Fullname, schemaString, SchemaFormat.Avro, CancellationToken.None);//.ConfigureAwait(false);

        var msg = new EventData()
        {
            Data = BinaryData.FromBytes(data),
            //ContentType = "avro/binary+"+schemaProperties.Id,
            ContentType = Encoding.UTF8.GetString(result, 0, result.Length)
        };
        T deserialized = (T)serializer.Deserialize(msg, typeof(T),CancellationToken.None);
        
        //return (Task<T>) Convert.ChangeType(deserialized, typeof(T));
        return Task.FromResult(deserialized);
    }
    

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        throw new NotImplementedException();
    }
    
}