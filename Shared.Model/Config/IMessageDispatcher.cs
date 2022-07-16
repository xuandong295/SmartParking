using System;
using RabbitMQ.Client;


namespace Shared.Model.Config
{
    public interface IMessageDispatcher : IDisposable
    {
        bool Enqueue<TMessage>(string queueName, TMessage message);
        bool Enqueue<TMessage>(TMessage message);
        TMessage Dequeue<TMessage>(string queueName);
        TMessage Dequeue<TMessage>();
        IModel GetChannel();
        IModel GetChannel(string queueName);
        IModel DeleteQueue(string queueName);
    }
}
