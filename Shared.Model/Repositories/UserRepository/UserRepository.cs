using Microsoft.EntityFrameworkCore;
using Nest;
using Shared.Common.Helpers;
using Shared.Model.Entities.EF;
using Shared.Model.Entities.ElasticSearchModel;
using Shared.Model.Entities.Model;
using Shared.Model.Persistence;
using Shared.Model.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Repositories.UserRepository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly DataContext DataContext;
        IPersistenceFactory PersistenceFactory;
        public UserRepository(DataContext dataContext, IPersistenceFactory persistenceFactory) : base(dataContext)
        {
            PersistenceFactory = persistenceFactory;
            DataContext = dataContext;
        }
        public async Task<User> LoginAsync(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName)||string.IsNullOrEmpty(password))
            {
                return null;
            }
            var currentUser = await DataContext.tblUser.Where(o => o.Password == password).Where(o=>o.UserName == userName).FirstOrDefaultAsync();
            if (currentUser!=null)
            {
                return (User)currentUser;

            }
            return null;
        }
        public async Task<long> CaculateParkingFee(string licensePlate)
        {
            using (var elasticSearchClient = PersistenceFactory.GetElasticSearchClient())
            {
                //var cars = new List<User>();
                //// get list resources
                //QueryContainerDescriptor<object> query = new QueryContainerDescriptor<object>();
                ////Đoạn này phải check state = 1 nữa
                //query.Bool(b => b.
                //           Filter(mu => mu
                //                   .Term(t => t
                //                      .Field("licensePlateNumber.keyword")
                //                      .Value(licensePlate)
                //                      )
                //        )
                //    );
                //var aggs = new AggregationContainerDescriptor<object>();
                //aggs.Terms("getTimeIn", t => t.Field("timeIn")
                //        .Order(o => o.KeyDescending())
                //        .Size(1))
                //    .Terms("getBalance", t => t.Field("timeIn")
                //    .Order(o => o.KeyDescending())
                //        .Size(1));// only get latest time)
                //var aggResponse = await elasticSearchClient.CustomizeAggregationAsync("1122", query, aggs, 0);
                //if (aggResponse != null)
                //{
                //    var termTimeIn = aggResponse.Aggregations.Terms("getTimeIn").Buckets;
                //    foreach (var item in termTimeIn)
                //    {
                //        var timeIn = item.Key;
                //        // string to long
                //    }
                //    var termBalance = aggResponse.Aggregations.Terms("getBalance").Buckets;
                //    foreach (var item in termBalance)
                //    {
                //        var balanceIn = item.Key;
                //    }
                //}
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

                var resourceResponseHits = await elasticSearchClient.GetAllDocumentsInIndexAsync("1122", q, "1m", 5000);
                foreach (var hit in resourceResponseHits)
                {
                    var jsonStr = JsonHelper.Serialize(hit.Source);
                    var resource = JsonHelper.Deserialize<Car>(jsonStr);
                    cars.Add(resource);
                }
                var currentCarParking = cars.OrderByDescending(o => o.TimeIn).ToList()[0];

                //bên trên có thể get all rồi lọc lại theo trường timeIn?

                // tính tiền
                DateTime timeOut = DateTime.Now;
                // check thời gian hiện tại trừ đi thời gian gửi
                var timeIn = UnixTimestamp.UnixTimestampToDateTime(currentCarParking.TimeIn);
                var totalTimeInParkingSpace = timeOut - timeIn;
                long totalMoney = totalTimeInParkingSpace.Days * 50 + totalTimeInParkingSpace.Hours * 20 + totalTimeInParkingSpace.Minutes * 1;
                //Sau đó tính tiền dựa theo thời gian
                var user = await DataContext.tblUser.Where(o => o.LisencePlateNumber.Contains(licensePlate)).FirstOrDefaultAsync();
                user.Balance -= totalMoney;
                //////// nếu tiền âm trả lại messenge
                //if (user.Balance < 0) return;
                DataContext.tblUser.Update(user);
                await DataContext.SaveChangesAsync();
                //Tiến hành update dữ liệu
                //Update tiền vào bảng user ở sql
                //update là xóa ở bảng all, add ở bảng phụ
                QueryContainerDescriptor<object> qu = new QueryContainerDescriptor<object>();
                qu.Bool(b => b.
                            Filter(mu => mu
                                    .Term(t => t
                                        .Field("licensePlateNumber.keyword")
                                        .Value(licensePlate)
                                        )
                        )
                    );
                var deleteDocInSmt = elasticSearchClient.DeleteByQueryAsync("all", qu);
                currentCarParking.Status = 0;
                await elasticSearchClient.IndexOneAsync(currentCarParking, timeOut.ToString("d"));
                return totalMoney;
            }
        }
    }
}
