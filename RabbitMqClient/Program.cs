using System;
using System.IO;
using RabbitMQ.Client;
using RabbitMqCommon;

namespace RabbitMqClient
{
    class Program
    {
        private static readonly string HostName = "localhost";
        private static readonly string QueueName = "DocQueue";

        static void Main(string[] args)
        {
            var service = new RabbitMqService(HostName);
            var channel = service.GetModel(QueueName);

            var contentBody = File.ReadAllBytes(@"./Resources/cat.jpg");

            channel.BasicPublish(
                exchange: "",
                routingKey: QueueName,
                basicProperties: null,
                body: contentBody);

            Console.WriteLine($" [x] Sent message. Length of content: {contentBody.Length}");

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}