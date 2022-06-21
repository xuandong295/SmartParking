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
        //get all information in out all the time
        public async Task<InternalAPIResponseCode> GetCarInformation(string licensePlate)
        {
            using (var elasticSearchClient = PersistenceFactory.GetElasticSearchClient())
            {
                var cars = new List<Car>();
                // get list resources
                QueryContainerDescriptor<object> q = new QueryContainerDescriptor<object>();
                q.Bool(b => b.
                           Filter(mu => mu
                                   .Term(t => t
                                      .Field("licensePlateNumber.keyword")
                                      .Value(licensePlate)
                                      )
                        )
                    );

                var resourceResponseHits = await elasticSearchClient.GetAllDocumentsInIndexAsync("DataGeneral", q, "1m", 5000);
                foreach (var hit in resourceResponseHits)
                {
                    var jsonStr = JsonHelper.Serialize(hit.Source);
                    var resource = JsonHelper.Deserialize<Car>(jsonStr);
                    cars.Add(resource);
                }
                var currentCarParking = cars.OrderByDescending(o => o.TimeIn).ToList()[0];
                return new InternalAPIResponseCode
                {
                    Code = APICodeResponse.SUCCESSED_CODE,
                    Message = MessageAPIResponse.OK,
                    Data = currentCarParking
                };
            }
        }
        public async Task<List<Car>> GetAllCarParkingOnDate(string date)
        {
            using (var elasticSearchClient = PersistenceFactory.GetElasticSearchClient())
            {
                var cars = new List<Car>();
                // get list resources
                QueryContainerDescriptor<object> q = new QueryContainerDescriptor<object>();
                q.MatchAll();
                var resourceResponseHits = await elasticSearchClient.GetAllDocumentsInIndexAsync(date, q, "1m", 5000);
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
            car.Id = Guid.NewGuid().ToString();
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
                //on day, remain, DataGeneral
                await elasticSearchClient.IndexOneAsync(car, "CarRemain");
                await elasticSearchClient.IndexOneAsync(car, "DataGeneral");
            }
        }
    }
}
