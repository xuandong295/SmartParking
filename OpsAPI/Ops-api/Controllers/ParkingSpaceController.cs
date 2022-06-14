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
using static Shared.Model.ConstantHelper.ConstantHelper;

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
        [HttpGet]
        public async Task<IActionResult> GetAllParkingSpaces()
        {
            var parkingSpace = await ParkingSpaceRepository.GetAllParkingSpacesAsync();
            if (parkingSpace == null)
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
                Data = parkingSpace
            });
        }
        [HttpGet]
        [Route("space")]
        public async Task<IActionResult> GetParkingSpace(string id)
        {
            var parkingSpace = await ParkingSpaceRepository.GetParkingSpaceAsync(id);
            if (parkingSpace == null)
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
                Data = parkingSpace
            });
        }
        [HttpPut]
        public async Task<IActionResult> UpdateParkingSpace(string id, int status)
        {
            var parkingSpace = await ParkingSpaceRepository.UpdateParkingSpaceAsync(id, status);
            return Ok(new InternalAPIResponseCode
            {
                Code = APICodeResponse.SUCCESSED_CODE,
                Message = MessageAPIResponse.OK,
                Data = parkingSpace
            });
        }
    }
}
