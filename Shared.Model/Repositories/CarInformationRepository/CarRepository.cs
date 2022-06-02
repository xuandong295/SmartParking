using Nest;
using Shared.Common.Helpers;
using Shared.Model.Entities.EF;
using Shared.Model.Entities.ElasticSearchModel;
using Shared.Model.Persistence;
using Shared.Model.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.Model.ConstantHelper.ConstantHelper;

namespace Shared.Model.Repositories.CarInformationRepository
{
    public class CarRepository : BaseRepository<Car>, ICarRepository
    {
        IPersistenceFactory PersistenceFactory;
        public CarRepository(DataContext dataContext, IPersistenceFactory persistenceFactory) : base(dataContext)
        {
            PersistenceFactory = persistenceFactory;
        }
        public async Task<InternalAPIResponseCode> GetCarInformation(string id, long time)
        {
            using (var elasticSearchClient = PersistenceFactory.GetElasticSearchClient())
            {
                var cloudAccountDbIdQuery = elasticSearchClient.BuildFilterdTermQuery("cloudAccountDbId.keyword", id);
                //Query severity
                var querySeverity = new QueryContainerDescriptor<object>();
                querySeverity.Bool(b => b.
                            Filter(
                                    mu => mu.Term(t => cloudAccountDbIdQuery)
                            )
                            );
                var aggs = new AggregationContainerDescriptor<object>();
                aggs.Terms("scanTime", v => v.Field("scanTime"));
                var aggResponse = await elasticSearchClient.CustomizeAggregationAsync("1234", querySeverity, aggs, 0);
                if (aggResponse != null)
                {
                    var smt = aggResponse.Hits;
                    var scanTimes = aggResponse.Aggregations.Terms("scanTime").Buckets;
                    foreach (var scanTime in scanTimes)
                    {
                        var b = scanTime.Key;
                    }
                }
                return new InternalAPIResponseCode
                {
                    Code = APICodeResponse.SUCCESSED_CODE,
                    Message = MessageAPIResponse.QUERY_SUCCESSED,
                    Data = querySeverity
                };

            }
        }
        public async Task<List<Car>> GetAllCarParkingOnDate(DateTime dateTime, string id)
        {
            using (var elasticSearchClient = PersistenceFactory.GetElasticSearchClient())
            {
                var cars = new List<Car>();
                // get list resources
                QueryContainerDescriptor<object> q = new QueryContainerDescriptor<object>();
                q.Bool(b => b.
                           Filter(mu => mu
                                   .Term(t => t
                                      .Field("carId.keyword")
                                      .Value(id)
                                      )
                        )
                    );
                var resourceResponseHits = await elasticSearchClient.GetAllDocumentsInIndexAsync(dateTime.ToString(), q, "1m", 5000);
                foreach (var hit in resourceResponseHits)
                {
                    var jsonStr = JsonHelper.Serialize(hit.Source);
                    var resource = JsonHelper.Deserialize<Car>(jsonStr);
                    cars.Add(resource);
                }
                return cars;
            }
        }
        public async Task InputCarIndex(Car car)
        {
            string timeNow = DateTime.Now.Date.ToString("d");
            using (var elasticSearchClient = PersistenceFactory.GetElasticSearchClient())
            {
                // check index is exist?
                if (!elasticSearchClient.IsExistedIndex(timeNow))
                {
                    elasticSearchClient.CreateIndexAutoMapping<Car>(timeNow);
                }
                await elasticSearchClient.IndexOneAsync(car, timeNow);
                //create 2 document in here
                //on day, still in car parking, 1 cái nữa là tổng hợp tất cả
                // nên làm cả 3 không?

            }
        }
    }
}
