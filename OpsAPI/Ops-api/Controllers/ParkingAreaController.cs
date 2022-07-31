using Microsoft.AspNetCore.Authorization;
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
using static Shared.Model.ConstantHelper.ConstantHelper;

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
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllParkingAreas()
        {
            var parkingAreas = await ParkingAreaRepository.GetAllParkingAreasAsync();
            return Ok(new InternalAPIResponseCode
            {
                Code = APICodeResponse.SUCCESSED_CODE,
                Message = MessageAPIResponse.OK,
                Data = parkingAreas
            });
        }
        [HttpGet]
        [Route("area")]
        public async Task<IActionResult> GetParkingArea(string id)
        {
            var parkingArea = await ParkingAreaRepository.GetParkingAreaAsync(id);
            if (parkingArea == null)
            {
                return Ok(new InternalAPIResponseCode
                {
                    Code = APICodeResponse.FAILED_CODE,
                    Message = MessageAPIResponse.RESOURCE_NOT_FOUND,
                    Data = null
                });
            }
            return Ok(new InternalAPIResponseCode
            {
                Code = APICodeResponse.SUCCESSED_CODE,
                Message = MessageAPIResponse.OK,
                Data = parkingArea
            });
        }
    }
}
