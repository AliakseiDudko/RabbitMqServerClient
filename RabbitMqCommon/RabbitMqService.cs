using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqCommon
{
    public class QueueService
    {
        private readonly IModel channel;
        private readonly string queueName;

        private readonly IDictionary<string, ChunkedMessage> chunkedMessages = new Dictionary<string, ChunkedMessage>();

        public event ChunkedMessageEventHandler ChunkedMessageReceived;
        public delegate void ChunkedMessageEventHandler(object sender, ChunkedMessageEventArgs e);

        public QueueService(string hostName, string queueName)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = hostName
            };
            var connection = connectionFactory.CreateConnection();

            channel = connection.CreateModel();
            channel.QueueDeclare(queueName, false, false, false, null);

            this.queueName = queueName;
        }

        public void Send(byte[] content, int chunkSize = 4096)
        {
            var contentLength = content.Length;
            var currentOffset = 0;
            var currentContent = new byte[0];

            var contentIdentifier = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var lastMessage = false;

            while (!lastMessage)
            {
                if (currentOffset + chunkSize < contentLength)
                {
                    currentContent = new byte[chunkSize];
                    Array.Copy(content, currentOffset, currentContent, 0, chunkSize);
                }
                else
                {
                    var remainingSize = contentLength - currentOffset;
                    currentContent = new byte[remainingSize];
                    Array.Copy(content, currentOffset, currentContent, 0, remainingSize);
                    lastMessage = true;
                }

                var basicProperties = channel.CreateBasicProperties();
                basicProperties.Persistent = true;
                basicProperties.Headers = new Dictionary<string, object>
                {
                    { nameof(contentIdentifier), contentIdentifier },
                    { nameof(contentLength), contentLength },
                    { "contentOffset", currentOffset }
                };

                channel.BasicPublish(string.Empty, queueName, basicProperties, currentContent);

                currentOffset += chunkSize;
            }
        }

        public void Listen()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += ReceiveMessage;

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        private void ReceiveMessage(object sender, BasicDeliverEventArgs e)
        {
            var contentIdentifier = Encoding.UTF8.GetString((e.BasicProperties.Headers["contentIdentifier"] as byte[]));
            var contentLength = Convert.ToInt32(e.BasicProperties.Headers["contentLength"]);
            var contentOffset = Convert.ToInt32(e.BasicProperties.Headers["contentOffset"]);

            ChunkedMessage chunkedMessage;
            if (chunkedMessages.ContainsKey(contentIdentifier))
            {
                chunkedMessage = chunkedMessages[contentIdentifier];
            }
            else
            {
                chunkedMessage = new ChunkedMessage
                {
                    Content = new byte[contentLength],
                    ReceivedContent = 0
                };
                chunkedMessages.Add(contentIdentifier, chunkedMessage);
            }

            Array.Copy(e.Body, 0, chunkedMessage.Content, contentOffset, e.Body.Length);
            chunkedMessage.ReceivedContent += e.Body.Length;

            if (chunkedMessage.ReceivedContent == contentLength)
            {
                if (ChunkedMessageReceived != null)
                {
                    var messageEvent = new ChunkedMessageEventArgs(chunkedMessage.Content);
                    ChunkedMessageReceived(this, messageEvent);
                }

                chunkedMessages.Remove(contentIdentifier);
            }
        }
    }
}
