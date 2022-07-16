using Shared.Model.ElasticSearch;
using Microsoft.Extensions.Logging;
using Shared.Model.Config;

namespace Shared.Model.Persistence
{
    public class PersistenceFactory : IPersistenceFactory
    {
        private RabbitMqConfiguration rabbitMqConfiguration;

        public ElasticSearchConfiguration ElasticSearchConfig { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        public RabbitMqConfiguration RabbitConfig { get; set; }
        public AppConfig RabbitMqQueues { get; set; }
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

        public PersistenceFactory
    (
    ElasticSearchConfiguration elasticSearchConfiguration,
    ILoggerFactory loggerFactory,
    RabbitMqConfiguration rabbitMqConfiguration
    )
        {
            ElasticSearchConfig = elasticSearchConfiguration;
            LoggerFactory = loggerFactory;
            RabbitConfig = rabbitMqConfiguration;
        }
        public PersistenceFactory
    (
    ElasticSearchConfiguration elasticSearchConfiguration,
    ILoggerFactory loggerFactory,
    RabbitMqConfiguration rabbitMqConfiguration,
    AppConfig rabbitMqQueues
    )
        {
            ElasticSearchConfig = elasticSearchConfiguration;
            LoggerFactory = loggerFactory;
            RabbitMqQueues = rabbitMqQueues;
            RabbitConfig = rabbitMqConfiguration;

        }
        public PersistenceFactory()
        {

        }

        public IElasticSearchClient GetElasticSearchClient()
        {
            return new ElasticSearchClient(ElasticSearchConfig, LoggerFactory);
        }
        public IMessageDispatcher GetMessageDispatcher()
        {
            return new RabbitMqDispatcher(RabbitConfig, LoggerFactory);
        }
        public IAppConfig GetAppConfig()
        {
            return RabbitMqQueues;
        }
    }
}
