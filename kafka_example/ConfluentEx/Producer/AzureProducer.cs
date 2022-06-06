using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Data.SchemaRegistry;
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Azure.Data.SchemaRegistry.ApacheAvro;

public class AzureProducer
{
    public static async Task Run()
    {
        var connectionString = "Endpoint=sb://dddddd.servicebus.windows.net/;SharedAccessKeyName=mypolicy;SharedAccessKey=ALRAd;EntityPath=d";
        var eventHubName = "my_topic";
        var schemaRegistryEndpoint = "dddddd.servicebus.windows.net";
        var schemaGroup = "AvroSchm";
        
        // Create a producer client that you can use to send events to an event hub
        var producerClient = new EventHubProducerClient(connectionString, eventHubName); 

        // Create a schema registry client that you can use to serialize and validate data.  
        var schemaRegistryClient = new SchemaRegistryClient(schemaRegistryEndpoint, credential: new DefaultAzureCredential());

        // Create a batch of events 
        using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

        // Create a new order object using the generated type/class 'Order'. 
        var sampleOrder = new Order { id = "12345", amount = 55.99, description = "This is a sample order." };
   
        using var memoryStream = new MemoryStream();
        // Create an Avro object serializer using the Schema Registry client object. 
        var producerSerializer = new SchemaRegistryAvroSerializer(schemaRegistryClient, schemaGroup, new SchemaRegistryAvroSerializerOptions { AutoRegisterSchemas = true });

        // Serialize events data to the memory stream object. 
        //producerSerializer.Serialize(memoryStream, sampleOrder, typeof(Order), CancellationToken.None);

        byte[] _memoryStreamBytes;
        _memoryStreamBytes = memoryStream.ToArray();

        // Create event data with serialized data and add it to an event batch. 
        eventBatch.TryAdd(new EventData(_memoryStreamBytes));

        // Send serilized event data to event hub. 
        await producerClient.SendAsync(eventBatch);
        Console.WriteLine("A batch of 1 order has been published.");
    }

}