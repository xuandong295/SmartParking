

using FPT.akaSAFE.Shared.Model.ElasticSearch;

namespace Shared.Model.Persistence
{
    /// <summary>
    /// Defines the persistence factory for getting repository when needed
    /// </summary>
    public interface IPersistenceFactory
    {
        IElasticSearchClient GetElasticSearchClient();

    }
}
