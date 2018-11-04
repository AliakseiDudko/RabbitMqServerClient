using RabbitMQ.Client;

namespace RabbitMqCommon
{
    public class RabbitMqService
    {
        private readonly IConnection connection;

        public RabbitMqService(string hostName)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = hostName
            };

            connection = connectionFactory.CreateConnection();
        }

        public IModel GetModel(string queueName)
        {
            var model = connection.CreateModel();
            model.QueueDeclare(queueName, false, false, false, null);

            return model;
        }
    }
}
