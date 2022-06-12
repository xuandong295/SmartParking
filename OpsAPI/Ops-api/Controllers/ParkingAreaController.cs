using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Model.Entities.EF;
using Shared.Model.Persistence;
using Shared.Model.Repositories.ParkingAreaRepository;
using Shared.Model.Repositories.ParkingSpaceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ops_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingAreaController : ControllerBase
    {
        public readonly DataContext _context;
        private IPersistenceFactory PersistenceFactory;
        private IParkingAreaRepository ParkingAreaRepository { get; set; }
        private IConfiguration _config;
        public ParkingAreaController(DataContext context, IPersistenceFactory persistenceFactory, IParkingAreaRepository parkingAreaRepository, IConfiguration config)
        {
            _context = context;
            PersistenceFactory = persistenceFactory;
            ParkingAreaRepository = parkingAreaRepository;
            _config = config;
        }
    }
}
