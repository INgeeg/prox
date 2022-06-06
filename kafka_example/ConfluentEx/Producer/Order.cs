using System;
using Microsoft.Azure.Amqp.Serialization;
using Microsoft.Azure.Data.SchemaRegistry.ApacheAvro;

public class Order
{
    public string description { get; set; }
    public double amount { get; set; }
    public string id { get; set; }
}