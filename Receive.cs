﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace receiver;

class Receive
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {

            channel.QueueDeclare(
                queue: "Hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
                Thread.Sleep(TimeSpan.FromSeconds(3));
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple : false);
                Console.WriteLine($"Acknowledged message: {message}");
            };

            channel.BasicConsume(
                queue: "Hello",
                autoAck: false,
                consumer: consumer
                );

            Console.WriteLine("Done processing consumer...");
            Console.ReadLine();
        }
    }
}