using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMqCommon;

namespace RabbitMqClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new RabbitMqService("localhost");
            var connection = service.GetConnection();
            var channel = connection.CreateModel();
            service.SetupQueue(channel, "DocQueue");

            string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "",
                routingKey: "DocQueue",
                basicProperties: null,
                body: body);

            Console.WriteLine(" [x] Sent {0}", message);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}