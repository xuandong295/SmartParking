using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Config
{
    public class RabbitMqConfiguration
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string VHost { get; set; }

        public string QueueName { get; set; }
        public string Exchange { get; set; } = string.Empty;
        public bool Durable { get; set; } = true;
        public bool Exclusive { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        public IDictionary<string, object> Arguments { get; set; } = null;

        public RabbitMqConfiguration() { }
        public RabbitMqConfiguration(string Server, string Port, string Username, string Password, string QueueName, string VHost)
        {
            this.Server = Server;
            this.Port = Port;
            this.Username = Username;
            this.Password = Password;
            this.QueueName = QueueName;
            this.VHost = VHost;
        }
    }

}
