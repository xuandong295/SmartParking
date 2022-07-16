using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Entities.Model
{
    public class RabbitMQMessage
    {
        public string Type { get; set; }
        public long Time { get; set; }
    }
}
