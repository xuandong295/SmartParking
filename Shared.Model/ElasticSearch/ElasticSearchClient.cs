using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json.Linq;
using Shared.Model.Config;

namespace FPT.akaSAFE.Shared.Model.ElasticSearch
{
    public class ElasticSearchClient : IElasticSearchClient, IDisposable
    {
        public ElasticClient ElasticClient { get; set; }
        public ElasticSearchConfiguration Configuration { get; }
        public ILoggerFactory Logger { get; set; }

        public ElasticSearchClient(ElasticSearchConfiguration configuration, ILoggerFactory loggerFactory)
        {

            this.ElasticClient = CreateConnection(configuration.ConnectionSettings);
            this.Logger = loggerFactory;
        }

        private ElasticClient CreateConnection(ConnectionSettings connectionSettings)
        {
            var elasticClient = new ElasticClient(connectionSettings);
            return elasticClient;
        }

        public void Dispose()
        {
            //do nothing
        }

        //add one document to Elasticsearch
        public bool IndexOneWithId(Object obj, string index, int id)
        {
            try
            {
                this.ElasticClient.Index(obj, i => i
                    .Index(index)
                    .Id(id)
                    );
            }
            catch (Exception)
            {
                //log
                return false;
            }
            return true;
        }

        public async System.Threading.Tasks.Task<bool> IndexOneAsync(Object obj, string index)
        {
            try
            {
                await this.ElasticClient.IndexAsync(obj, i => i
                    .Index(index)
                    );
            }
            catch (Exception)
            {
                //log
                return false;
            }
            return true;
        }
        public bool IndexOneDynamicWithId(object obj, string index, int id)
        {
            try
            {
                this.ElasticClient.Index<dynamic>(obj, i => i
                    .Index(index)
                    .Id(id)
                    );
            }
            catch (Exception)
            {
                //log
                return false;
            }
            return true;
        }

        public bool IndexOneDynamic(object obj, string index)
        {
            try
            {
                this.ElasticClient.Index<dynamic>(obj, i => i
                    .Index(index)
                    );
            }
            catch (Exception)
            {
                //log
                return false;
            }
            return true;
        }
        public bool BulkOne(object obj, string index)
        {
            try
            {
                this.ElasticClient.Bulk(b => b
                    .Index<object>(i => i
                        .Index(index)
                        .Document(obj))
                    );
            }
            catch (Exception)
            {
                //log
                return false;
            }
            return true;
        }

        public bool IndexManyDynamic(List<object> obj, string index)
        {
            try
            {
                this.ElasticClient.IndexMany<dynamic>(obj, index);
            }
            catch (Exception)
            {
                //log
                return false;
            }
            return true;
        }
        public bool IndexManyWithRefresh<T>(List<T> obj, string index) where T : class
        {
            try
            {
                this.ElasticClient.IndexMany<T>(obj, index);
                this.ElasticClient.Indices.Refresh(index);
            }
            catch (Exception)
            {
                //log
                return false;
            }
            return true;
        }

        public bool IndexMany<T>(List<T> obj, string index) where T : class
        {
            try
            {
                this.ElasticClient.Bulk(b => b
                    .Index(index)
                    .IndexMany(obj));
            }
            catch (Exception)
            {
                //log
                return false;
            }
            return true;
        }

        public ISearchResponse<object> Search(string index, QueryContainer query, int responseSize)
        {
            var searchResponse = this.ElasticClient.Search<Object>(s => s
                    .Index(index)
                    .Size(responseSize)
                    .Query(q =>
                    {
                        return query;
                    })
                );

            return searchResponse;
        }

        public async System.Threading.Tasks.Task<ISearchResponse<object>> SearchExcludeFieldsAsync(string index, QueryContainer query, string[] excludeFields, int responseSize)
        {
            var searchResponse = await this.ElasticClient.SearchAsync<Object>(s => s
                    .Index(index)
                    .Size(responseSize)
                    .Query(q =>
                    {
                        return query;
                    })
                    .Source(so => so
                        .Excludes(e => e
                           .Fields(excludeFields)
                            )
                        )
                );
            return searchResponse;
        }

        public ISearchResponse<object> SearchToGetSpecificFields(string index, QueryContainer query, int responseSize, string[] responseFields)
        {
            var searchResponse = this.ElasticClient.Search<Object>(s => s
                    .Index(index)
                    .Size(responseSize)
                    .Source(sf => sf
                        .Includes(i => i
                        .Fields(responseFields)
                        )
                    )
                    .Query(q =>
                    {
                        return query;
                    })
                );
            return searchResponse;
        }

        public List<IHit<object>> GetAllDocumentsInIndex(string indexName, QueryContainer query, string scrollTimeout = "1m", int scrollSize = 5000)
        {
            var isSuccess = false;
            var retry = 5;
            while (isSuccess == false && retry > 0)
            {
                retry--;
                ISearchResponse<object> initialResponse = this.ElasticClient.Search<object>
                (scr => scr.Index(indexName)
                     .From(0)
                     .Take(scrollSize)
                      .Query(q =>
                      {
                          return query;
                      })
                     .Scroll(scrollTimeout));
                isSuccess = CheckESQueryResponse(initialResponse);

                var results = new List<IHit<object>>();
                if (initialResponse != null)
                {
                    if (!initialResponse.IsValid || string.IsNullOrEmpty(initialResponse.ScrollId))
                        throw new Exception(initialResponse.ServerError.Error.Reason);

                    if (initialResponse.Hits.Count > 0)
                        results.AddRange(initialResponse.Hits);

                    string scrollid = initialResponse.ScrollId;
                    bool isScrollSetHasData = true;
                    while (isScrollSetHasData)
                    {
                        ISearchResponse<object> loopingResponse = this.ElasticClient.Scroll<object>(scrollTimeout, scrollid);
                        if (loopingResponse.IsValid)
                        {
                            results.AddRange(loopingResponse.Hits);
                            scrollid = loopingResponse.ScrollId;
                        }
                        isScrollSetHasData = loopingResponse.Hits.Count > 0 ? true : false;
                    }

                    this.ElasticClient.ClearScroll(new ClearScrollRequest(scrollid));
                    return results;
                }
            }
            return new List<IHit<object>>();
        }

        public async System.Threading.Tasks.Task<List<IHit<object>>> GetAllDocumentsInIndexAsync(string indexName, QueryContainer query, string scrollTimeout = "1m", int scrollSize = 5000)
        {
            var isSuccess = false;
            var retry = 5;
            while (isSuccess == false && retry > 0)
            {
                retry--;
                try
                {
                    ISearchResponse<object> initialResponse = await this.ElasticClient.SearchAsync<object>
                    (scr => scr.Index(indexName)
                         .From(0)
                         .Take(scrollSize)
                          .Query(q =>
                          {
                              return query;
                          })
                         .Scroll(scrollTimeout));
                    var json = ElasticClient.RequestResponseSerializer.SerializeToString(query);
                    isSuccess = CheckESQueryResponse(initialResponse);

                    var results = new List<IHit<object>>();
                    if (initialResponse != null)
                    {
                        if (!initialResponse.IsValid || string.IsNullOrEmpty(initialResponse.ScrollId))
                            throw new Exception(initialResponse.ServerError.Error.Reason);

                        if (initialResponse.Hits.Count > 0)
                            results.AddRange(initialResponse.Hits);

                        string scrollid = initialResponse.ScrollId;
                        bool isScrollSetHasData = true;
                        while (isScrollSetHasData)
                        {
                            ISearchResponse<object> loopingResponse = this.ElasticClient.Scroll<object>(scrollTimeout, scrollid);
                            if (loopingResponse.IsValid)
                            {
                                results.AddRange(loopingResponse.Hits);
                                scrollid = loopingResponse.ScrollId;
                            }
                            isScrollSetHasData = loopingResponse.Hits.Count > 0 ? true : false;
                        }

                        this.ElasticClient.ClearScroll(new ClearScrollRequest(scrollid));
                        return results;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return new List<IHit<object>>();
        }

        public async System.Threading.Tasks.Task<List<IHit<object>>> GetAllDocumentsIncludeSpecificFieldsAsync(string indexName, QueryContainer query, string[] fields, string scrollTimeout = "1m", int scrollSize = 5000)
        {
            var isSuccess = false;
            var retry = 5;
            while (isSuccess == false && retry > 0)
            {
                retry--;
                try
                {
                    ISearchResponse<object> initialResponse = await this.ElasticClient.SearchAsync<object>
                    (scr => scr.Index(indexName)
                         .From(0)
                         .Take(scrollSize)
                          .Source(sf => sf
                            .Includes(i => i
                            .Fields(fields)
                            )
                            )
                          .Query(q =>
                          {
                              return query;
                          })
                         .Scroll(scrollTimeout));

                    isSuccess = CheckESQueryResponse(initialResponse);

                    var results = new List<IHit<object>>();
                    if (initialResponse != null)
                    {
                        if (!initialResponse.IsValid || string.IsNullOrEmpty(initialResponse.ScrollId))
                            throw new Exception(initialResponse.ServerError.Error.Reason);

                        if (initialResponse.Hits.Count > 0)
                            results.AddRange(initialResponse.Hits);
                        string scrollid = initialResponse.ScrollId;
                        bool isScrollSetHasData = true;
                        while (isScrollSetHasData)
                        {
                            ISearchResponse<object> loopingResponse = this.ElasticClient.Scroll<object>(scrollTimeout, scrollid);
                            if (loopingResponse.IsValid)
                            {
                                results.AddRange(loopingResponse.Hits);
                                scrollid = loopingResponse.ScrollId;
                            }
                            isScrollSetHasData = loopingResponse.Hits.Count > 0 ? true : false;
                        }

                        this.ElasticClient.ClearScroll(new ClearScrollRequest(scrollid));
                        return results;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.Message);
                }
            }
            return new List<IHit<object>>();
        }

        public List<IHit<object>> GetAllDocumentsIncludeSpecificFields(string indexName, QueryContainer query, string[] fields, string scrollTimeout = "1m", int scrollSize = 5000)
        {
            var isSuccess = false;
            var retry =5;
            while (isSuccess == false && retry > 0)
            {
                retry--;
                try
                {
                    ISearchResponse<object> initialResponse = this.ElasticClient.Search<object>
                    (scr => scr.Index(indexName)
                         .From(0)
                         .Take(scrollSize)
                          .Source(sf => sf
                            .Includes(i => i
                            .Fields(fields)
                            )
                            )
                          .Query(q =>
                          {
                              return query;
                          })
                         .Scroll(scrollTimeout));

                    isSuccess = CheckESQueryResponse(initialResponse);

                    var results = new List<IHit<object>>();
                    if (initialResponse != null)
                    {
                        if (!initialResponse.IsValid || string.IsNullOrEmpty(initialResponse.ScrollId))
                            throw new Exception(initialResponse.ServerError.Error.Reason);

                        if (initialResponse.Hits.Count > 0)
                            results.AddRange(initialResponse.Hits);
                        string scrollid = initialResponse.ScrollId;
                        bool isScrollSetHasData = true;
                        while (isScrollSetHasData)
                        {
                            ISearchResponse<object> loopingResponse = this.ElasticClient.Scroll<object>(scrollTimeout, scrollid);
                            if (loopingResponse.IsValid)
                            {
                                results.AddRange(loopingResponse.Hits);
                                scrollid = loopingResponse.ScrollId;
                            }
                            isScrollSetHasData = loopingResponse.Hits.Count > 0 ? true : false;
                        }

                        this.ElasticClient.ClearScroll(new ClearScrollRequest(scrollid));
                        return results;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.Message);
                }
            }
            return new List<IHit<object>>();
        }

        public async System.Threading.Tasks.Task<List<IHit<object>>> GetAllDocumentsExcludeSpecificFieldsAsync(string indexName, QueryContainer query, string[] fields, string scrollTimeout = "1m", int scrollSize = 5000)
        {
            var isSuccess = false;
            var retry =5;
            while (isSuccess == false && retry > 0)
            {
                retry--;
                ISearchResponse<object> initialResponse = await this.ElasticClient.SearchAsync<object>
              (scr => scr.Index(indexName)
                   .From(0)
                   .Take(scrollSize)
                    .Source(so => so
                      .Excludes(e => e
                         .Fields(fields)
                          )
                      )
                    .Query(q =>
                    {
                        return query;
                    })
                   .Scroll(scrollTimeout));

                isSuccess = CheckESQueryResponse(initialResponse);

                var results = new List<IHit<object>>();
                if (initialResponse != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(initialResponse.ScrollId))
                            //do nothing because no next page
                            if (!initialResponse.IsValid)
                            {
                                throw new Exception();
                            }
                        if (initialResponse.Hits.Count > 0)
                            results.AddRange(initialResponse.Hits);

                        string scrollid = initialResponse.ScrollId;
                        bool isScrollSetHasData = true;
                        while (isScrollSetHasData)
                        {
                            ISearchResponse<object> loopingResponse = this.ElasticClient.Scroll<object>(scrollTimeout, scrollid);
                            if (loopingResponse.IsValid)
                            {
                                results.AddRange(loopingResponse.Hits);
                                scrollid = loopingResponse.ScrollId;
                            }
                            isScrollSetHasData = loopingResponse.Hits.Count > 0 ? true : false;
                        }
                        this.ElasticClient.ClearScroll(new ClearScrollRequest(scrollid));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                        Console.WriteLine(ex.Message);
                    }

                    return results;
                }
            }
            return new List<IHit<object>>();
        }

        public async System.Threading.Tasks.Task<Tuple<int?, List<IHit<object>>>> GetAllDocumentsExcludeSpecificFieldsAndResponseStatusAsync(string indexName, QueryContainer query, string[] fields, string scrollTimeout = "1m", int scrollSize = 5000)
        {
            ISearchResponse<object> initialResponse = await this.ElasticClient.SearchAsync<object>
                (scr => scr.Index(indexName)
                     .From(0)
                     .Take(scrollSize)
                      .Source(so => so
                        .Excludes(e => e
                           .Fields(fields)
                            )
                        )
                      .Query(q =>
                      {
                          return query;
                      })
                     .Scroll(scrollTimeout));

            var status = initialResponse.ApiCall.HttpStatusCode;

            var results = new List<IHit<object>>();
            if (initialResponse != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(initialResponse.ScrollId))
                        // do nothing because no next page
                        if (!initialResponse.IsValid)
                        {
                            throw new Exception();
                        }
                    if (initialResponse.Hits.Count > 0)
                        results.AddRange(initialResponse.Hits);

                    string scrollid = initialResponse.ScrollId;
                    bool isScrollSetHasData = true;
                    while (isScrollSetHasData)
                    {
                        ISearchResponse<object> loopingResponse = this.ElasticClient.Scroll<object>(scrollTimeout, scrollid);
                        status = loopingResponse.ApiCall.HttpStatusCode;
                        if (loopingResponse.IsValid)
                        {
                            results.AddRange(loopingResponse.Hits);
                            scrollid = loopingResponse.ScrollId;
                        }
                        isScrollSetHasData = loopingResponse.Hits.Count > 0 ? true : false;
                    }
                    this.ElasticClient.ClearScroll(new ClearScrollRequest(scrollid));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.Message);
                }

                return Tuple.Create(status, results);
            }

            return Tuple.Create(status, new List<IHit<object>>());
        }

        public async System.Threading.Tasks.Task<Tuple<int?, List<IHit<object>>>> GetAllDocumentsIncludeAndExcludeSpecificFieldsAndResponseStatusAsync(string indexName, QueryContainer query, string[] includeFields, string[] excludeFields, string scrollTimeout = "1m", int scrollSize = 5000)
        {
            ISearchResponse<object> initialResponse = await this.ElasticClient.SearchAsync<object>
                (scr => scr.Index(indexName)
                     .From(0)
                     .Take(scrollSize)
                      .Source(so => so
                        .Excludes(e => e
                           .Fields(excludeFields)
                            )
                        .Includes(i => i
                        .Fields(includeFields)
                        )
                        )
                      .Query(q =>
                      {
                          return query;
                      })
                     .Scroll(scrollTimeout));

            var status = initialResponse.ApiCall.HttpStatusCode;

            var results = new List<IHit<object>>();
            if (initialResponse != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(initialResponse.ScrollId))
                        // do nothing because no next page
                        if (!initialResponse.IsValid)
                        {
                            throw new Exception();
                        }
                    if (initialResponse.Hits.Count > 0)
                        results.AddRange(initialResponse.Hits);

                    string scrollid = initialResponse.ScrollId;
                    bool isScrollSetHasData = true;
                    while (isScrollSetHasData)
                    {
                        ISearchResponse<object> loopingResponse = this.ElasticClient.Scroll<object>(scrollTimeout, scrollid);
                        status = loopingResponse.ApiCall.HttpStatusCode;
                        if (loopingResponse.IsValid)
                        {
                            results.AddRange(loopingResponse.Hits);
                            scrollid = loopingResponse.ScrollId;
                        }
                        isScrollSetHasData = loopingResponse.Hits.Count > 0 ? true : false;
                    }
                    this.ElasticClient.ClearScroll(new ClearScrollRequest(scrollid));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.Message);
                }

                return Tuple.Create(status, results);
            }

            return Tuple.Create(status, new List<IHit<object>>());
        }

        public List<IHit<object>> GetAllDocumentsExcludeSpecificFields(string indexName, QueryContainer query, string[] fields, string scrollTimeout = "1m", int scrollSize = 5000)
        {
            ISearchResponse<object> initialResponse = this.ElasticClient.Search<object>
                (scr => scr.Index(indexName)
                     .From(0)
                     .Take(scrollSize)
                      .Source(so => so
                        .Excludes(e => e
                           .Fields(fields)
                            )
                        )
                      .Query(q =>
                      {
                          return query;
                      })
                     .Scroll(scrollTimeout));


            var results = new List<IHit<object>>();
            if (initialResponse != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(initialResponse.ScrollId))
                        // do nothing because no next page
                        if (!initialResponse.IsValid)
                        {
                            throw new Exception();
                        }
                    if (initialResponse.Hits.Count > 0)
                        results.AddRange(initialResponse.Hits);

                    string scrollid = initialResponse.ScrollId;
                    bool isScrollSetHasData = true;
                    while (isScrollSetHasData)
                    {
                        ISearchResponse<object> loopingResponse = this.ElasticClient.Scroll<object>(scrollTimeout, scrollid);
                        if (loopingResponse.IsValid)
                        {
                            results.AddRange(loopingResponse.Hits);
                            scrollid = loopingResponse.ScrollId;
                        }
                        isScrollSetHasData = loopingResponse.Hits.Count > 0 ? true : false;
                    }
                    this.ElasticClient.ClearScroll(new ClearScrollRequest(scrollid));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.Message);
                }


                return results;
            }

            return new List<IHit<object>>();
        }

        public ISearchResponse<object> SearchBySortField(string index, string field, QueryContainer query, int responseSize, SortOrder sortOrder)
        {
            var searchResponse = this.ElasticClient.Search<Object>(s => s
                    .Index(index)
                    .Size(responseSize)
                    .Query(q =>
                    {
                        return query;
                    })
                    .Sort(so => so
                        .Field(f => f
                            .Field(field)
                            .Order(sortOrder)
                            )
                        )
                );
            return searchResponse;
        }

        public ISearchResponse<object> TermAggregate(string index, QueryContainer query, string termAggName, string field, int aggSize, int responseSize = 10000)
        {
            var searchResponse = this.ElasticClient.Search<Object>(s => s
                    .Index(index)
                    .Aggregations(agg => agg
                        .Terms(termAggName, t => t
                        .Field(field)
                        .Size(aggSize)
                        )
                    )
                    .Query(q =>
                    {
                        return query;
                    })
                    .Size(responseSize)
            );
            return searchResponse;
        }

        public ISearchResponse<object> AggregateNestedObject(string index, QueryContainer query, string nestedName, string path, string termAggName, string field, int aggSize, int responseSize)
        {
            var searchResponse = this.ElasticClient.Search<Object>(s => s
                    .Index(index)
                    .Aggregations(agg => agg
                        .Nested(nestedName, n => n
                            .Path(path)
                            .Aggregations(aa => aa
                            .Terms(termAggName, t => t
                                .Field(field)
                                .Size(aggSize)
                                )
                            )
                        )
                    )
                    .Query(q =>
                    {
                        return query;
                    })
                    .Size(responseSize)
            );
            return searchResponse;
        }

        public ISearchResponse<object> CustomizeAggregation(string index, QueryContainer query, AggregationContainerDescriptor<object> aggs, int responseSize)
        {
            var searchResponse = this.ElasticClient.Search<Object>(s => s
                    .Index(index)
                    .Aggregations(agg =>
                    {
                        return aggs;
                    }
                    )
                    .Query(q =>
                    {
                        return query;
                    })
                    .Size(responseSize)
            );
            return searchResponse;
        }

        public async Task<ISearchResponse<object>> CustomizeAggregationAsync(string index, QueryContainer query, AggregationContainerDescriptor<object> aggs, int responseSize)
        {
            var searchResponse = await this.ElasticClient.SearchAsync<Object>(s => s
                    .Index(index)
                    .Aggregations(agg =>
                    {
                        return aggs;
                    }
                    )
                    .Query(q =>
                    {
                        return query;
                    })
                    .Size(responseSize)
            );
            return searchResponse;
        }

        public async Task<ISearchResponse<object>> CustomizeAggregationBySortedFieldAsync(string index, QueryContainer query, AggregationContainerDescriptor<object> aggs, string sortedField, SortOrder sortOrder, int responseSize)
        {
            var searchResponse = await this.ElasticClient.SearchAsync<object>(s => s
                    .Index(index)
                    .Aggregations(agg =>
                    {
                        return aggs;
                    }
                    )
                    .Query(q =>
                    {
                        return query;
                    })
                    .Sort(so => so
                    .Field(f => f
                        .Field(sortedField)
                        .Order(sortOrder)
                        )
                    )
                    .Size(responseSize)
            );
            return searchResponse;
        }

        public ISearchResponse<object> CustomizeAggregationExcludeFields(string index, QueryContainer query, AggregationContainerDescriptor<object> aggs, string[] fields, int responseSize)
        {
            var searchResponse = this.ElasticClient.Search<Object>(s => s
                    .Index(index)
                    .Source(so => so
                        .Excludes(e => e
                           .Fields(fields)
                            )
                        )
                    .Aggregations(agg =>
                    {
                        return aggs;
                    }
                    )
                    .Query(q =>
                    {
                        return query;
                    })
                    .Size(responseSize)
            );
            return searchResponse;
        }

        public ISearchResponse<object> SumAggregate(string index, QueryContainer query, string sumAggName, string field, int responseSize)
        {
            var searchResponse = this.ElasticClient.Search<Object>(s => s
                    .Index(index)
                    .Aggregations(agg => agg
                        .Sum(sumAggName, su => su
                            .Field(field))
                    )
                    .Query(q =>
                    {
                        return query;
                    })
                    .Size(responseSize)
            );
            return searchResponse;
        }

        public async System.Threading.Tasks.Task DeleteByQueryAsync(string index, QueryContainer query)
        {
            var retry = 5;
            var isSuccess = false;

            while(isSuccess == false && retry > 0)
            {
                retry--;
                var deleteByQueryResponse = await ElasticClient.DeleteByQueryAsync<object>(d => d
                .Index(index)
                .Query(q =>
                {
                    return query;
                })
                
            );
                isSuccess = deleteByQueryResponse.IsValid;
            }
        }
        public void CreateIndex(string index)
        {
            if (!ElasticClient.Indices.Exists(index).Exists)
            {
                ElasticClient.Indices.Create(index);
            }
        }

        public async Task CreateIndexAsync(string index)
        {
            if (!ElasticClient.Indices.Exists(index).Exists)
            {
                await ElasticClient.Indices.CreateAsync(index);
            }
        }
        public void CreateIndexAutoMapping<T>(string index) where T : class
        {
            if (!ElasticClient.Indices.Exists(index).Exists)
            {
                ElasticClient.Indices.Create(index, c => c
                    .Map<T>(mm => mm.AutoMap())
                );
            }
        }
        public void DeleteIndex(string index)
        {
            ElasticClient.Indices.Delete(index);
        }

        public async System.Threading.Tasks.Task DeleteIndexAsync(string index)
        {
            await ElasticClient.Indices.DeleteAsync(index);
        }

        public async System.Threading.Tasks.Task DeleteIndexesExceptAsync(string indexPattern, List<string> indexesException)
        {
            foreach(var index in indexesException)
            {
                indexPattern += $",-{index}*";
            }
            var result = await this.ElasticClient.LowLevel.Indices.DeleteAsync<StringResponse>(indexPattern);
            Console.WriteLine(result.Body);
        }

        public async System.Threading.Tasks.Task UpdateSourceAsync(string index, QueryContainer query, string sourceScriptInPainless, string lang)
        {
            await ElasticClient.UpdateByQueryAsync<object>(u => u
                    .Index(index)
                    .Query(q =>
                    {
                        return query;
                    }
                    )
                    .Script(s => s
                        .Source(sourceScriptInPainless)
                        .Lang(lang)
                    )
                );
        }

        public bool IsExistedIndex(string index)
        {
            return ElasticClient.Indices.Exists(index).Exists;
        }

        public TermsQuery BuildFilterdTermsQuery(string field, List<string> filterValues)
        {
            var query = new TermsQuery();
            if (filterValues != null)
            {
                query = new TermsQuery
                {
                    Field = field,
                    Terms = filterValues
                };
            }
            return query;
        }

        public TermQuery BuildFilterdTermQuery(string field, string value)
        {
            var query = new TermQuery();
            if (!String.IsNullOrEmpty(value))
            {
                query = new TermQuery
                {
                    Field = field,
                    Value = value
                };
            }
            return query;
        }

        private bool CheckESQueryResponse(ISearchResponse<object> response)
        {
            if (response.IsValid == false)
            {
                if (response.DebugInformation.Contains("Invalid NEST response"))
                {
                    return false;
                }
                if (response.ServerError.Status == 404)
                {
                    return true; //ignore this because retry will get the same response
                }
                if (response.ServerError.Status == 400)
                {
                    return true; //ignore this because retry will get the same response
                }
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task UpdateSourceByIdAsync(string index, QueryContainer query, string sourceScriptInPainless, Dictionary<string, object> param, string lang)
        {
            var response = await ElasticClient.UpdateByQueryAsync<object>(u => u
                    .Index(index)
                    .Query(q =>
                    {
                        return query;
                    }
                    )
                    .Script(s => s
                        .Source(sourceScriptInPainless)
                        .Params(param)
                        .Lang(lang)
                    )
                );
        }
    }
}
