using Microsoft.Extensions.Configuration;
using Azure.Storage.Queues;
using System;
using Newtonsoft.Json;
using System.Text;

namespace Messaging
{
    public class MessageQueue : IMessageQueue
    {
        private readonly QueueClient _queue;
        public MessageQueue(IConfiguration configuration)
        {
            var endpoint = configuration.GetValue<string>("MessageQueue:Url");
            var name = configuration.GetValue<string>("MessageQueue:Name");

            _queue = new QueueClient(endpoint, name);
            _queue.CreateIfNotExists();
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : Message
        {
            var json = JsonConvert.SerializeObject(message);
            var bytes = Encoding.UTF8.GetBytes(json);

            _queue.SendMessage(Convert.ToBase64String(bytes));             
        }
    }
}
