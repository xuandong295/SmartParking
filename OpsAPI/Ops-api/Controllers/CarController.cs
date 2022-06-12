using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Model.Entities.EF;
using Shared.Model.Persistence;
using Shared.Model.Repositories.CarInformationRepository;
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
    public class CarController : ControllerBase
    {
        public readonly DataContext _context;
        private IPersistenceFactory PersistenceFactory;
        private ICarRepository CarRepository { get; set; }
        private IConfiguration _config;
        public CarController(DataContext context, IPersistenceFactory persistenceFactory, ICarRepository carRepository, IConfiguration config)
        {
            _context = context;
            PersistenceFactory = persistenceFactory;
            CarRepository = carRepository;
            _config = config;
        }
        [HttpGet]
        [Route("car-infor")]
        public async Task<IActionResult> GetCarInformation(string licensePlate)
        {
            try
            {
                var carInfor = await CarRepository.GetCarInformation(licensePlate);
                return Ok(new InternalAPIResponseCode
                {
                    Code = APICodeResponse.SUCCESSED_CODE,
                    Message = MessageAPIResponse.OK,
                    Data = carInfor
                });
            }
            catch (Exception)
            {
                return Ok(new InternalAPIResponseCode
                {
                    Code = APICodeResponse.FAILED_CODE,
                    Message = MessageAPIResponse.ERROR,
                    Data = null
                });
            }
            
        }
        [HttpGet]
        [Route("car-infor-on-date")]
        public async Task<IActionResult> GetCarInformationParkingOnDate(string date)
        {
            try
            {
                var carInforOnDate = await CarRepository.GetAllCarParkingOnDate(date);
                return Ok(new InternalAPIResponseCode
                {
                    Code = APICodeResponse.SUCCESSED_CODE,
                    Message = MessageAPIResponse.OK,
                    Data = carInforOnDate
                });
            }
            catch (Exception)
            {
                return Ok(new InternalAPIResponseCode
                {
                    Code = APICodeResponse.FAILED_CODE,
                    Message = MessageAPIResponse.ERROR,
                    Data = null
                });
            }

        }
    }
}
