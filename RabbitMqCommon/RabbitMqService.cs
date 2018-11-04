using RabbitMQ.Client;

namespace RabbitMqCommon
{
    public class RabbitMqService
    {
        private readonly string hostName;

        public RabbitMqService(string hostName)
        {
            this.hostName = hostName;
        }

        public IConnection GetConnection()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = hostName
            };

            return connectionFactory.CreateConnection();
        }

        public void SetupQueue(IModel channel, string queueName)
        {
            channel.QueueDeclare(queueName, true, false, false, null);
        }
    }
}
