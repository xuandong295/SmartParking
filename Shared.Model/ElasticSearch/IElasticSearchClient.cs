using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FPT.akaSAFE.Shared.Model.ElasticSearch
{
    public interface IElasticSearchClient : IDisposable
    {
        ISearchResponse<object> CustomizeAggregation(string index, QueryContainer query, AggregationContainerDescriptor<object> aggs, int responseSize);
        Task<ISearchResponse<object>> CustomizeAggregationAsync(string index, QueryContainer query, AggregationContainerDescriptor<object> aggs, int responseSize);
        Task<ISearchResponse<object>> CustomizeAggregationBySortedFieldAsync(string index, QueryContainer query, AggregationContainerDescriptor<object> aggs, string sortedField, SortOrder sortOrder, int responseSize);
        bool IndexOneWithId(Object obj, string index, int id);
        Task<bool> IndexOneAsync(Object obj, string index);
        ISearchResponse<Object> Search(string index, QueryContainer query, int responseSize);
        ISearchResponse<Object> SearchToGetSpecificFields(string index, QueryContainer query, int responseSize, string[] responseFields);
        ISearchResponse<object> SearchBySortField(string index, string field, QueryContainer query, int responseSize, SortOrder sortOrder);
        bool IndexManyDynamic(List<object> obj, string index);
        bool IndexOneDynamicWithId(object obj, string index, int id);
        bool IndexOneDynamic(object obj, string index);
        void CreateIndex(string index);
        void CreateIndexAutoMapping<T>(string index) where T : class;
        void DeleteIndex(string index);
        Task DeleteIndexAsync(string index);
        Task DeleteByQueryAsync(string index, QueryContainer query);
        bool IsExistedIndex(string index);
        ISearchResponse<object> AggregateNestedObject(string index, QueryContainer query, string nestedName, string path, string termAggName, string field, int aggSize, int responseSize);
        ISearchResponse<object> SumAggregate(string index, QueryContainer query, string sumAggName, string field, int responseSize);
        ISearchResponse<object> TermAggregate(string index, QueryContainer query, string termAggName, string field, int aggSize, int responseSize);
        bool IndexManyWithRefresh<T>(List<T> obj, string index) where T : class;
        System.Threading.Tasks.Task<ISearchResponse<object>> SearchExcludeFieldsAsync(string index, QueryContainer query, string[] excludeFields, int responseSize);
        Task UpdateSourceAsync(string index, QueryContainer query, string sourceScriptInPainless, string lang);
        Task<List<IHit<object>>> GetAllDocumentsInIndexAsync(string indexName, QueryContainer query, string scrollTimeout, int scrollSize);
        Task<List<IHit<object>>> GetAllDocumentsIncludeSpecificFieldsAsync(string indexName, QueryContainer query, String[] fileds, string scrollTimeout = "1m", int scrollSize = 5000);
        Task<List<IHit<object>>> GetAllDocumentsExcludeSpecificFieldsAsync(string indexName, QueryContainer query, String[] fileds, string scrollTimeout = "1m", int scrollSize = 5000);
        List<IHit<object>> GetAllDocumentsInIndex(string indexName, QueryContainer query, string scrollTimeout, int scrollSize);
        List<IHit<object>> GetAllDocumentsIncludeSpecificFields(string indexName, QueryContainer query, String[] fileds, string scrollTimeout = "1m", int scrollSize = 5000);
        List<IHit<object>> GetAllDocumentsExcludeSpecificFields(string indexName, QueryContainer query, String[] fileds, string scrollTimeout = "1m", int scrollSize = 5000);
        ISearchResponse<object> CustomizeAggregationExcludeFields(string index, QueryContainer query, AggregationContainerDescriptor<object> aggs, string[] fields, int responseSize);
        TermsQuery BuildFilterdTermsQuery(string field, List<string> filterValues);
        TermQuery BuildFilterdTermQuery(string field, string value);
        Task UpdateSourceByIdAsync(string index, QueryContainer query, string sourceScriptInPainless, Dictionary<string, object> param, string lang);
    }
}
