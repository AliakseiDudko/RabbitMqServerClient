using RabbitMQ.Client;

namespace RabbitMqCommon
{
    public class RabbitMqService
    {
        public IConnection GetConnection(string hostName)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = hostName
            };

            return connectionFactory.CreateConnection();
        }

        public IModel GetModel(IConnection connection, string queueName)
        {
            var model = connection.CreateModel();
            model.QueueDeclare(queueName, false, false, false, null);

            return model;
        }
    }
}
