namespace RabbitMqCommon
{
    internal class ChunkedMessage
    {
        public byte[] Content { get; set; }
        public int ReceivedContent { get; set; }
    }
}
