using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Shared.Common.Helpers;

namespace Shared.Model.Config
{
    public class RabbitMqDispatcher : IMessageDispatcher
    {
        private static readonly object RootLock = new object();
        private static readonly ConcurrentDictionary<string, IConnection> Connections = new ConcurrentDictionary<string, IConnection>();

        public RabbitMqConfiguration Configuration { get; }
        public ILoggerFactory Logger { get; }

        public RabbitMqDispatcher(RabbitMqConfiguration configuration, ILoggerFactory logger)
        {
            Configuration = configuration;
            Logger = logger;
        }


        public bool Enqueue<TMessage>(string queueName, TMessage message)
        {
            return AddQueueItem(queueName, message);
        }

        public bool Enqueue<TMessage>(TMessage message)
        {
            if (string.IsNullOrEmpty(Configuration.QueueName)) return false;
            return AddQueueItem(Configuration.QueueName, message);
        }

        public TMessage Dequeue<TMessage>(string queueName)
        {
            return PopFromQueue<TMessage>(queueName);
        }

        public TMessage Dequeue<TMessage>()
        {
            if (string.IsNullOrEmpty(Configuration.QueueName)) return default(TMessage);
            return PopFromQueue<TMessage>(Configuration.QueueName);
        }

        public IModel GetChannel()
        {
            if (string.IsNullOrEmpty(Configuration.QueueName)) return null;
            var channel = CreateChannel(Configuration.QueueName);
            channel.QueueDeclare(
                queue: Configuration.QueueName,
                durable: Configuration.Durable,
                exclusive: Configuration.Exclusive,
                autoDelete: Configuration.AutoDelete,
                arguments: Configuration.Arguments
            );

            return channel;
        }

        public IModel DeleteQueue(string queueName)
        {
            if (string.IsNullOrEmpty(Configuration.QueueName)) return null;
            var channel = CreateChannel(Configuration.QueueName);
            channel.QueueDeleteNoWait(queueName, false, false);  //it is possible to delete a queue only if it is empty:
            return channel;
        }

        public IModel GetChannel(string queueName)
        {
            var channel = CreateChannel(queueName);
            channel.QueueDeclare(
                queue: queueName,
                durable: Configuration.Durable,
                exclusive: Configuration.Exclusive,
                autoDelete: Configuration.AutoDelete,
                arguments: Configuration.Arguments
            );

            return channel;
        }

        private bool AddQueueItem(string queueName, object message)
        {
            try
            {
                //Dictionary<String, Object> args = new Dictionary<String, Object>();
                //args["expires"] = 3800000; // set expire for more than 1 hour if no one use that queue anymore: 1hour + 3 mins
                using (var channel = CreateChannel(queueName))
                {
                    lock (channel)
                    {
                        channel.QueueDeclare(queue: queueName,
                            durable: Configuration.Durable,
                            exclusive: Configuration.Exclusive,
                            autoDelete: Configuration.AutoDelete,
                            arguments: Configuration.Arguments
                        );

                        var body = JsonHelper.Serialize(message);
                        var bodyData = Encoding.UTF8.GetBytes(body);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        channel.BasicPublish(
                            exchange: Configuration.Exchange,
                            routingKey: queueName,
                            basicProperties: properties,
                            body: bodyData
                        );

                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                //this.Logger.($"Enqueue - Message {exception.Message} - Stacktrace: {exception.StackTrace}");
                return false;
            }
        }

        private TMessage PopFromQueue<TMessage>(string queueName)
        {
            try
            {

                using (var channel = CreateChannel(queueName))
                {
                    lock (channel)
                    {
                        channel.QueueDeclare(queue: queueName,
                            durable: Configuration.Durable,
                            exclusive: Configuration.Exclusive,
                            autoDelete: Configuration.AutoDelete,
                            arguments: Configuration.Arguments);

                        var result = channel.BasicGet(queueName, false);
                        if (result == null)
                        {
                            return default(TMessage);
                        }

                        var body = result.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        // deserialize
                        var item = JsonHelper.Deserialize<TMessage>(message);

                        channel.BasicAck(result.DeliveryTag, false);

                        return item;
                    }
                }
            }
            catch (Exception exception)
            {
                //this.Logger.LogError($"PopFromQueue - Message: {exception.Message} - Stacktrace: {exception.StackTrace}");
                return default(TMessage);
            }
        }

        private IConnection CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                UserName = this.Configuration.Username,
                Password = this.Configuration.Password,
                VirtualHost = this.Configuration.VHost,
                HostName = this.Configuration.Server,
                Port = int.Parse(this.Configuration.Port)
            };

            var connection = factory.CreateConnection();

            //connection.AutoClose = false;
            return connection;
        }

        private IModel CreateChannel(string queueName)
        {
            lock (RootLock)
            {
                try
                {
                    IConnection connection = null;
                    if (Connections.ContainsKey(queueName))
                    {
                        connection = Connections[queueName];
                    }

                    if (connection == null || !connection.IsOpen)
                    {
                        connection = CreateConnection();
                        Connections[queueName] = connection;
                    }

                    return connection.CreateModel();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    Console.WriteLine(exception.StackTrace);
                    //this.Logger.LogError($"GetChannel. Message: {exception.Message} - Stacktrace: {exception.StackTrace}");
                    throw;
                }
            }
        }

        public void Dispose()
        {
            // do nothing
        }
    }

}
