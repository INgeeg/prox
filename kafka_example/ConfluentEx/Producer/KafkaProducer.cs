using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avro;
using Avro.Generic;
using Avro.IO;
using Avro.Specific;
using AvroSpecific;
using Azure.Core;
using Azure.Data.SchemaRegistry;
using Azure.Identity;
using Azure.Messaging;
using Azure.Messaging.EventHubs;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Azure.Data.SchemaRegistry.ApacheAvro;
using Encoder = Avro.IO.Encoder;

public class KafkaProducer
{
    public static async Task Run()
    {
        string brokerList = "ddd.servicebus.windows.net:9093";
        string connectionString = "Endpoint=sb://ddd.servicebus.windows.net/;SharedAccessKeyName=mypolicy;SharedAccessKey=sdf=;EntityPath=sdf";
        string topic = "my_topic";
        string caCertLocation = "";
        string consumerGroup = "$Default";
       string schemaRegistryUrl = "ddd.servicebus.windows.net";
        string schemaGroup = "AvroSchm";
        
        // var schemaRegistryConfig = new SchemaRegistryConfig
        // {
        //     Url = schemaRegistryUrl,
        //     BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo, 
        //     BasicAuthUserInfo = "7bcbe235-537e-4db5-96c8-8a39d7675a5b:nNa7Q~kRk_CYkjxMAvSQtkA2oe.DIKnCkqBTM"
        // };
        
        
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = "https://dsgfds-sdfsd.westus2.azure.confluent.cloud",
            BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo, 
            BasicAuthUserInfo = "sdgd:mKMsb7UhFA+g+SCxz82iminAb6/vgG2S30WN60"
        };
        //
        
        try
        {
            var config = new ProducerConfig
            {
                BootstrapServers = brokerList,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = connectionString,
                
                //SslCaLocation = cacertlocation,
                //Debug = "security,broker,protocol"        //Uncomment for librdkafka debugging information
            };
         
            var credentials = new ClientSecretCredential("6fecd0sd","7b75a5b","kqBTM");
            var valueSerializer = new KafkaAvroSerializer<User>(
                schemaRegistryUrl, 
                credentials,
                schemaGroup,
                autoRegisterSchemas: true);
        
      
            using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
            //using (var producer = new ProducerBuilder<long, string>(config)
            using (var producer = new ProducerBuilder<string, User>(config)
                       // .SetKeySerializer(Serializers.Int64)
                       // .SetValueSerializer(Serializers.Utf8)
                        .SetKeySerializer(Serializers.Utf8)
                        //.SetValueSerializer(new AvroSerializer<User>(schemaRegistry))
                        .SetValueSerializer(valueSerializer)
                        .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                        .Build()
                   )
            {
                Console.WriteLine("Sending 10 messages to topic: " + topic + ", broker(s): " + brokerList);
                for (int x = 0; x < 10; x++)
                {
                     User msg = new User { name = x.ToString(), favorite_color = "green", favorite_number = x, hourly_rate = new Avro.AvroDecimal(67.99) };
                     var deliveryReport = await producer.ProduceAsync(topic, new Message<string, User> { Key = x.ToString(), Value = msg });
                    //var msg = string.Format("Sample message #{0} sent at {1}", x, DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss.ffff"));
                    //var deliveryReport = await producer.ProduceAsync(topic, new Message<long, string> { Key = DateTime.UtcNow.Ticks, Value = msg });
                    
                    Console.WriteLine(string.Format("Message {0} sent (value: '{1}')", x, msg));
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(string.Format("Exception Occurred - {0}", e.Message));
        }
    }

}

public class KafkaAvroSerializer<T> : ISerializer<T>
{
    private readonly SchemaRegistryAvroSerializer serializer;
    private readonly SpecificSerializerImpl<T> serializer2;

    public KafkaAvroSerializer(string schemaRegistryUrl, TokenCredential credential, string schemaGroup, Boolean autoRegisterSchemas = false)
    {
        this.serializer = new SchemaRegistryAvroSerializer(
            new SchemaRegistryClient(
                schemaRegistryUrl,
                credential),
            schemaGroup,
            new SchemaRegistryAvroSerializerOptions()
            {
                AutoRegisterSchemas = autoRegisterSchemas
            });
        this.serializer2 = new SpecificSerializerImpl<T>(null, true, true, 1024, null);
    }

    public byte[] Serialize(T o, SerializationContext context)
    {
        if (o == null)
        {
            return null;
        }
        EventData eventData =  (EventData) serializer.Serialize(o, messageType: typeof(EventData));
        context.Headers.Add("schemaId", Encoding.ASCII.GetBytes(eventData.ContentType));
        
        return eventData.EventBody.ToArray();
        
        //Confluent Message =>   shemaid-message pauyload
        //Kafaka Message =>      message payload
    }
}
public class SpecificSerializerImpl<T>
  {
    private ISchemaRegistryClient schemaRegistryClient;
    private bool autoRegisterSchema;
    private bool useLatestVersion;
    private int initialBufferSize;
    private SubjectNameStrategyDelegate subjectNameStrategy;
    private Dictionary<System.Type, SpecificSerializerImpl<T>.SerializerSchemaData> multiSchemaData = new Dictionary<System.Type, SpecificSerializerImpl<T>.SerializerSchemaData>();
    private SpecificSerializerImpl<T>.SerializerSchemaData singleSchemaData;
    private SemaphoreSlim serializeMutex = new SemaphoreSlim(1);

    public SpecificSerializerImpl(
      ISchemaRegistryClient schemaRegistryClient,
      bool autoRegisterSchema,
      bool useLatestVersion,
      int initialBufferSize,
      SubjectNameStrategyDelegate subjectNameStrategy)
    {
      this.schemaRegistryClient = schemaRegistryClient;
      this.autoRegisterSchema = autoRegisterSchema;
      this.useLatestVersion = useLatestVersion;
      this.initialBufferSize = initialBufferSize;
      this.subjectNameStrategy = subjectNameStrategy;
      System.Type writerType = typeof (T);
      if (!(writerType != typeof (ISpecificRecord)))
        return;
      this.singleSchemaData = SpecificSerializerImpl<T>.ExtractSchemaData(writerType);
    }

    private static SpecificSerializerImpl<T>.SerializerSchemaData ExtractSchemaData(
      System.Type writerType)
    {
      SpecificSerializerImpl<T>.SerializerSchemaData schemaData = new SpecificSerializerImpl<T>.SerializerSchemaData();
      if (typeof (ISpecificRecord).IsAssignableFrom(writerType))
        schemaData.WriterSchema = (Avro.Schema) writerType.GetField("_SCHEMA", BindingFlags.Static | BindingFlags.Public).GetValue((object) null);
      else if (writerType.Equals(typeof (int)))
        schemaData.WriterSchema = Avro.Schema.Parse("int");
      else if (writerType.Equals(typeof (bool)))
        schemaData.WriterSchema = Avro.Schema.Parse("boolean");
      else if (writerType.Equals(typeof (double)))
        schemaData.WriterSchema = Avro.Schema.Parse("double");
      else if (writerType.Equals(typeof (string)))
        schemaData.WriterSchema = Avro.Schema.Parse("string");
      else if (writerType.Equals(typeof (float)))
        schemaData.WriterSchema = Avro.Schema.Parse("float");
      else if (writerType.Equals(typeof (long)))
        schemaData.WriterSchema = Avro.Schema.Parse("long");
      else if (writerType.Equals(typeof (byte[])))
      {
        schemaData.WriterSchema = Avro.Schema.Parse("bytes");
      }
      else
      {
        if (!writerType.Equals(typeof (Null)))
          throw new InvalidOperationException("AvroSerializer only accepts type parameters of int, bool, double, string, float, long, byte[], instances of ISpecificRecord and subclasses of SpecificFixed.");
        schemaData.WriterSchema = Avro.Schema.Parse("null");
      }
      schemaData.AvroWriter = new SpecificWriter<T>(schemaData.WriterSchema);
      schemaData.WriterSchemaString = schemaData.WriterSchema.ToString();
      return schemaData;
    }

    public async Task<byte[]> Serialize(string topic, T data, bool isKey)
    {
      byte[] array;
      try
      {
        await this.serializeMutex.WaitAsync().ConfigureAwait(false);
        SpecificSerializerImpl<T>.SerializerSchemaData currentSchemaData;
        try
        {
          if (this.singleSchemaData == null)
          {
            System.Type type = data.GetType();
            if (!this.multiSchemaData.TryGetValue(type, out currentSchemaData))
            {
              currentSchemaData = SpecificSerializerImpl<T>.ExtractSchemaData(type);
              this.multiSchemaData[type] = currentSchemaData;
            }
          }
          else
            currentSchemaData = this.singleSchemaData;
          string recordType = (string) null;
          if ((object) data is ISpecificRecord && ((ISpecificRecord) (object) data).Schema is RecordSchema)
            recordType = ((ISpecificRecord) (object) data).Schema.Fullname;
          string subject = this.subjectNameStrategy != null ? this.subjectNameStrategy(new SerializationContext(isKey ? MessageComponentType.Key : MessageComponentType.Value, topic), recordType) : (isKey ? this.schemaRegistryClient.ConstructKeySubjectName(topic, recordType) : this.schemaRegistryClient.ConstructValueSubjectName(topic, recordType));
          if (!currentSchemaData.SubjectsRegistered.Contains(subject))
          {
            if (this.useLatestVersion)
            {
              currentSchemaData.WriterSchemaId = new int?((await this.schemaRegistryClient.GetLatestSchemaAsync(subject).ConfigureAwait(false)).Id);
            }
            else
            {
              SpecificSerializerImpl<T>.SerializerSchemaData serializerSchemaData = currentSchemaData;
              int num;
              if (this.autoRegisterSchema)
                num = await this.schemaRegistryClient.RegisterSchemaAsync(subject, currentSchemaData.WriterSchemaString).ConfigureAwait(false);
              else
                num = await this.schemaRegistryClient.GetSchemaIdAsync(subject, currentSchemaData.WriterSchemaString).ConfigureAwait(false);
              serializerSchemaData.WriterSchemaId = new int?(num);
              serializerSchemaData = (SpecificSerializerImpl<T>.SerializerSchemaData) null;
            }
            currentSchemaData.SubjectsRegistered.Add(subject);
          }
          subject = (string) null;
        }
        finally
        {
          this.serializeMutex.Release();
        }
        using (MemoryStream output = new MemoryStream(this.initialBufferSize))
        {
          using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
          {
            output.WriteByte((byte) 0);
            binaryWriter.Write(IPAddress.HostToNetworkOrder(currentSchemaData.WriterSchemaId.Value));
            currentSchemaData.AvroWriter.Write(data, (Encoder) new BinaryEncoder((Stream) output));
            array = output.ToArray();
          }
        }
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
      }
      return array;
    }

    internal class SerializerSchemaData
    {
      private string writerSchemaString;
      private Avro.Schema writerSchema;
      /// <remarks>
      ///     A given schema is uniquely identified by a schema id, even when
      ///     registered against multiple subjects.
      /// </remarks>
      private int? writerSchemaId;
      private SpecificWriter<T> avroWriter;
      private HashSet<string> subjectsRegistered = new HashSet<string>();

      public HashSet<string> SubjectsRegistered
      {
        get => this.subjectsRegistered;
        set => this.subjectsRegistered = value;
      }

      public string WriterSchemaString
      {
        get => this.writerSchemaString;
        set => this.writerSchemaString = value;
      }

      public Avro.Schema WriterSchema
      {
        get => this.writerSchema;
        set => this.writerSchema = value;
      }

      public int? WriterSchemaId
      {
        get => this.writerSchemaId;
        set => this.writerSchemaId = value;
      }

      public SpecificWriter<T> AvroWriter
      {
        get => this.avroWriter;
        set => this.avroWriter = value;
      }
    }
  }
  
//return serializer2.Serialize("", o, true).Result;
        
// using (MemoryStream output = new MemoryStream())
// {
//     using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
//     {
//         output.WriteByte((byte) 0);
//         binaryWriter.Write(eventData.ContentType);
//         currentSchemaData.AvroWriter.Write(data, (Encoder) new BinaryEncoder((Stream) output));
//         array = output.ToArray();
//     }
// }

//return new byte[]{};
// EventData eventData =  (EventData) serializer.Serialize(o, messageType: typeof(EventData));
// var bf = new BinaryFormatter();
// using (var ms = new MemoryStream())
// {
//     bf.Serialize(ms, eventData);
//     return ms.ToArray();
// }
//return eventData.
//serializer.Serialize(stream, o.GetType(), typeof(MessageContent), CancellationToken.None);
//serializer.Serialize(o, messageType: typeof(T));
//return stream.ToArray();