using FPT.akaSAFE.Shared.Model.ElasticSearch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using Shared.Model.Config;
using Shared.Model.Entities.EF;
using Shared.Model.Entities.Model;
using Shared.Model.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public readonly DataContext _context;
        private IPersistenceFactory PersistenceFactory;
        public WeatherForecastController(DataContext context, IPersistenceFactory persistenceFactory)
        {
            _context = context;
            PersistenceFactory = persistenceFactory;
        }

        [HttpGet]
        public async Task<string> GetClasses()
        {
            GetResult<Class> result = new();
            try
            {
                List<Class> classes = new();
                classes = await _context.Class.ToListAsync();
                result.Config(1, classes, "Getted Successfully");
                string convert = JsonConvert.SerializeObject(result);
                return convert;
            }
            catch (Exception ex)
            {
                result.Config(0, null, ex.Message);
                string convert = JsonConvert.SerializeObject(result);
                return convert;
            }
        }

        [HttpPost]
        public async Task CreateSingle(Student student)
        {
            using (var elasticSearchClient = PersistenceFactory.GetElasticSearchClient())
            {
                elasticSearchClient.CreateIndex("1238");
            }
        }
    }
}
