using System;

namespace Messaging
{
    public abstract class Message
    {
        public string CorrelationId { get; set; }  

        public Message()
        {
            CorrelationId = Guid.NewGuid().ToString();
        }
    }
}
