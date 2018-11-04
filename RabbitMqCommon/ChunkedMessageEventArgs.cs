using System;

namespace RabbitMqCommon
{
    public class ChunkedMessageEventArgs : EventArgs
    {
        public byte[] Content { get; set; }

        public ChunkedMessageEventArgs(byte[] content)
        {
            Content = content;
        }
    }
}
