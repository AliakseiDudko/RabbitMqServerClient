using System;
using System.IO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqCommon;

namespace RabbitMqServer
{
    class Program
    {
        private static readonly string QueueName = "DocQueue";

        static void Main(string[] args)
        {
            var service = new RabbitMqService();
            var connection = service.GetConnection("localhost");
            var channel = service.GetModel(connection, QueueName);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                File.WriteAllBytes("cat.jpg", ea.Body);

                Console.WriteLine($" [x] Received. Length of content: {ea.Body.Length}");
            };

            channel.BasicConsume(queue: QueueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
