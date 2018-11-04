using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqCommon;

namespace RabbitMqServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new RabbitMqService("localhost");
            var connection = service.GetConnection();
            var channel = connection.CreateModel();
            service.SetupQueue(channel, "DocQueue");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };

            channel.BasicConsume(queue: "DocQueue",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
