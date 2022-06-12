using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Model.Entities.EF;
using Shared.Model.Persistence;
using Shared.Model.Repositories.ParkingSpaceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ops_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingSpaceController : ControllerBase
    {
        public readonly DataContext _context;
        private IPersistenceFactory PersistenceFactory;
        private IParkingSpaceRepository ParkingSpaceRepository { get; set; }
        private IConfiguration _config;
        public ParkingSpaceController(DataContext context, IPersistenceFactory persistenceFactory, IParkingSpaceRepository parkingSpaceRepository, IConfiguration config)
        {
            _context = context;
            PersistenceFactory = persistenceFactory;
            ParkingSpaceRepository = parkingSpaceRepository;
            _config = config;
        }

    }
}
