using FPT.akaSAFE.Shared.Model.ElasticSearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using Shared.Model.Config;
using Shared.Model.Entities.EF;
using Shared.Model.Entities.ElasticSearchModel;
using Shared.Model.Entities.Model;
using Shared.Model.Persistence;
using Shared.Model.Repositories.CarInformationRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Shared.Model.ConstantHelper.ConstantHelper;

namespace OpsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public readonly DataContext _context;
        private IPersistenceFactory PersistenceFactory;
        private ICarRepository CarRepository { get; set; }
        public WeatherForecastController(DataContext context, IPersistenceFactory persistenceFactory, ICarRepository carRepository)
        {
            _context = context;
            PersistenceFactory = persistenceFactory;
            CarRepository = carRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetClasses()
        {
            var result = await CarRepository.GetCarInformation("224f0f67-946e-4916-b5d6-8c502740546e", 123);
            return Ok(new InternalAPIResponseCode
            {
                Code = APICodeResponse.SUCCESSED_CODE,
                Message = MessageAPIResponse.OK,
                Data = result
            });
        }

        [HttpPost]
        public async Task CreateSingle(Student student)
        {
            using (var elasticSearchClient = PersistenceFactory.GetElasticSearchClient())
            {

                //elasticSearchClient.CreateIndex("1238");
                elasticSearchClient.CreateIndexAutoMapping<Car>("Index");
            }
        }
        [HttpPost]
        [Route("information")]
        public async Task CreateCarInformationIn(Car car)
        {
            await CarRepository.InputCarIndex(car);
        }
    }
}
