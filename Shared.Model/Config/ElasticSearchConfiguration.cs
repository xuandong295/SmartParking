using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Config
{
    public class ElasticSearchConfiguration
    {
        public ConnectionSettings ConnectionSettings { get; set; }

        public ElasticSearchConfiguration() { }

        public ElasticSearchConfiguration(ConnectionSettings connectionSettings)
        {
            ConnectionSettings = connectionSettings.DefaultDisableIdInference();
        }
    }
}
