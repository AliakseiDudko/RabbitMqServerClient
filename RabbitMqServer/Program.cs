using System;
using System.IO;
using RabbitMqCommon;

namespace RabbitMqServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new QueueService("localhost", "DocQueue");
            service.ChunkedMessageReceived += ChunkedMessageReceived;
            service.Listen();

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static void ChunkedMessageReceived(object sender, ChunkedMessageEventArgs e)
        {
            File.WriteAllBytes("cat.jpg", e.Content);

            Console.WriteLine($" [x] Received. Length of content: {e.Content.Length}");
        }
    }
}
