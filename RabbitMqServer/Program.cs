using System;
using System.IO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqCommon;

namespace RabbitMqServer
{
    class Program
    {
        private static readonly string HostName = "localhost";
        private static readonly string QueueName = "DocQueue";

        static void Main(string[] args)
        {
            var service = new RabbitMqService(HostName);
            var channel = service.GetModel(QueueName);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += ReceiveMessage;

            channel.BasicConsume(queue: QueueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static void ReceiveMessage(object sender, BasicDeliverEventArgs e)
        {
            File.WriteAllBytes("cat.jpg", e.Body);

            Console.WriteLine($" [x] Received. Length of content: {e.Body.Length}");
        }
    }
}
