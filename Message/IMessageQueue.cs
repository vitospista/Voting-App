namespace Messaging
{
    public interface IMessageQueue
    {
        void Publish<TMessage>(TMessage message) where TMessage : Message;
    }
}