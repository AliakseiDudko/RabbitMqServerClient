using System;
using System.IO;
using RabbitMqCommon;

namespace RabbitMqClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new QueueService("localhost", "DocQueue");

            var contentBody = File.ReadAllBytes(@"./Resources/cat.jpg");
            service.Send(contentBody);

            Console.WriteLine($" [x] Sent message. Length of content: {contentBody.Length}");

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}