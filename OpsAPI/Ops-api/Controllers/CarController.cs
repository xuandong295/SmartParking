using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nest;
using Shared.Common.Helpers;
using Shared.Model.Entities.EF;
using Shared.Model.Entities.ElasticSearchModel;
using Shared.Model.Entities.Model;
using Shared.Model.Persistence;
using Shared.Model.Repositories.CarInformationRepository;
using Shared.Model.Repositories.CarInformationRepository.DTO;
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
    public class CarController : ControllerBase
    {
        public readonly DataContext _context;
        private IPersistenceFactory PersistenceFactory;
        private ICarRepository CarRepository { get; set; }
        private IParkingAreaRepository ParkingAreaRepository { get; set; }
        private IConfiguration _config;
        public CarController(DataContext context, IPersistenceFactory persistenceFactory, IParkingAreaRepository parkingAreaRepository, ICarRepository carRepository, IConfiguration config)
        {
            _context = context;
            PersistenceFactory = persistenceFactory;
            CarRepository = carRepository;
            _config = config;
            ParkingAreaRepository = parkingAreaRepository;
        }
        [HttpGet]
        [Route("car-information")]
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
        [Route("car-history")]
        public async Task<IActionResult> GetCarHistoryInformation(string licensePlate)
        {
            try
            {
                var carInfor = await CarRepository.GetCarHistoryInformation(licensePlate);
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
        [Route("car-information-on-date")]
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
        [HttpPost]
        [Route("in")]
        public async Task<IActionResult> CreateCarComeInParkingSite(CarInDTO carIn)
        {
            Car newCar = new Car()
            {
                Id = Guid.NewGuid().ToString(),
                BackImageLink = carIn.BackImageLink,
                FrontImageLink = carIn.FrontImageLink,
                LicensePlateNumber = carIn.LicensePlateNumber,
                ParkingAreaId = carIn.ParkingAreaId,
                
                TimeIn = UnixTimestamp.DateTimeToUnixTimestamp(DateTime.Now),
                TimeOut = 0,
                Status = 1
            };
            //tạo thông tin trong db update parking area
            await CarRepository.InputCarIndex(newCar);
            var currentParkingSite = await ParkingAreaRepository.GetParkingAreaAsync(newCar.ParkingAreaId);
            await ParkingAreaRepository.UpdateParkingSpaceAsync(newCar.ParkingAreaId, currentParkingSite.Current + 1);
            //Send message to rabbitMQ
            using (var messageDispatcher = PersistenceFactory.GetMessageDispatcher())
            {
                var parkingAreaName = await ParkingAreaRepository.GetParkingAreaAsync(carIn.ParkingAreaId);
                var scheduleMessage = new RabbitMQMessage
                {
                    BackImageLink = carIn.BackImageLink,
                    FrontImageLink = carIn.FrontImageLink,
                    LicensePlateNumber = carIn.LicensePlateNumber,
                    ParkingAreaId = carIn.ParkingAreaId,
                    ParkingAreaName = parkingAreaName.Area,
                    TimeIn = UnixTimestamp.DateTimeToUnixTimestamp(DateTime.Now),
                    TimeOut = 0,
                    Status = 1
                };

                using (var rabbitMqQueues = PersistenceFactory.GetAppConfig())
                {
                    messageDispatcher.Enqueue<RabbitMQMessage>("wpf-manage-queue", scheduleMessage);
                }
            }
            return Ok(new InternalAPIResponseCode
            {
                Code = APICodeResponse.SUCCESSED_CODE,
                Message = MessageAPIResponse.OK,
                Data = null
            });
        }
        [HttpPost]
        [Route("out")]
        public async Task<IActionResult> CreateCarComeOutParkingSite(CarOutDTO carOut)
        {
            var currentCarParking = await CarRepository.GetCarInformation(carOut.LicensePlateNumber);
            Car oldCar = new Car()
            {
                Id = Guid.NewGuid().ToString(),
                BackImageLink = carOut.BackImageLink,
                FrontImageLink = carOut.FrontImageLink,
                LicensePlateNumber = currentCarParking.LicensePlateNumber,
                ParkingAreaId = currentCarParking.ParkingAreaId,
                TimeIn = currentCarParking.TimeIn,
                TimeOut = UnixTimestamp.DateTimeToUnixTimestamp(DateTime.Now),
                Status = 0
            };
            //tạo thông tin trong db update parking area
            await CarRepository.OutputCarIndex(oldCar);
            var currentParkingSite = await ParkingAreaRepository.GetParkingAreaAsync(oldCar.ParkingAreaId);
            await ParkingAreaRepository.UpdateParkingSpaceAsync(oldCar.ParkingAreaId, currentParkingSite.Current - 1);
            //Send message to rabbitMQ
            using (var messageDispatcher = PersistenceFactory.GetMessageDispatcher())
            {
                var parkingAreaName = await ParkingAreaRepository.GetParkingAreaAsync(oldCar.ParkingAreaId);
                var scheduleMessage = new RabbitMQMessage
                {
                    BackImageLink = carOut.BackImageLink,
                    FrontImageLink = carOut.FrontImageLink,
                    LicensePlateNumber = currentCarParking.LicensePlateNumber,
                    ParkingAreaId = currentCarParking.ParkingAreaId,
                    ParkingAreaName = parkingAreaName.Area,
                    TimeIn = currentCarParking.TimeIn,
                    TimeOut = UnixTimestamp.DateTimeToUnixTimestamp(DateTime.Now),
                    Status = 0
                };

                using (var rabbitMqQueues = PersistenceFactory.GetAppConfig())
                {
                    messageDispatcher.Enqueue<RabbitMQMessage>("wpf-manage-queue", scheduleMessage);
                }
            }
            return Ok(new InternalAPIResponseCode
            {
                Code = APICodeResponse.SUCCESSED_CODE,
                Message = MessageAPIResponse.OK,
                Data = null
            });
        }
    }
}
