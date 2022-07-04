using Shared.Model.ElasticSearch;
using Microsoft.Extensions.Logging;
using Shared.Model.Config;

namespace Shared.Model.Persistence
{
    public class PersistenceFactory : IPersistenceFactory
    {
        public ElasticSearchConfiguration ElasticSearchConfig { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }

        public PersistenceFactory
            (
            ILoggerFactory loggerFactory
            )
        {
            LoggerFactory = loggerFactory;
        }
        public PersistenceFactory
            (
            ElasticSearchConfiguration elasticSearchConfiguration,
            ILoggerFactory loggerFactory
            )
        {
            ElasticSearchConfig = elasticSearchConfiguration;
            LoggerFactory = loggerFactory;
        }

        public PersistenceFactory()
        {

        }

        public IElasticSearchClient GetElasticSearchClient()
        {
            return new ElasticSearchClient(ElasticSearchConfig, LoggerFactory);
        }
    }
}
